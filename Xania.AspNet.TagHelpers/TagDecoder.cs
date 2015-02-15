using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Xania.AspNet.TagHelpers
{
    public class TagDecoder : ITagDecoder
    {
        private readonly StringBuilder _chars;
        private readonly StringBuilder _name;
        // private DecoderStatus DecoderStatus { get; set; }
        public bool IsClosingTag { get; private set; }
        public bool IsSelfClosing { get; private set; }
        public bool Closed { get; set; }

        private readonly ICollection<TagAttribute> _attributes;
        private readonly Stack<Action<char>> _decoders;

        public TagDecoder()
        {
            _attributes = new Collection<TagAttribute>();
            _chars = new StringBuilder();
            _name = new StringBuilder();
            IsClosingTag = false;
            IsSelfClosing = false;

            _decoders = new Stack<Action<char>>();
            _decoders.Push(DecodeTagName);
            _decoders.Push(DecodeSeparator);
        }

        public void Append(char ch)
        {
            if (IsSelfClosing)
                return;

            _decoders.Peek()(ch);
        }

        private void DecodeTagName(char ch)
        {
            if (Char.IsWhiteSpace(ch))
            {
                _decoders.Clear();
                _decoders.Push(DecodeAttributeName);
                _decoders.Push(DecodeSeparator);
            }
            else if (ch == '/')
            {
                if (_name.Length > 0)
                {
                    _decoders.Clear();
                    _decoders.Push(DecodeAttributeName);
                }

                IsClosingTag = _name.Length == 0;
                _decoders.Push(DecodeSeparator);
            }
            else
            {
                _name.Append(ch);
            }
        }

        private void DecodeAttributeValue(char ch)
        {
            if (ch == '/')
            {
                IsSelfClosing = true;
                _attributes.Last().Value = _chars.ToString();
                _chars.Clear();
            }
            if (ch == '"' && _chars.Length == 0)
            {
                _decoders.Clear();
                _decoders.Push(DecodeConstantString);
            }
            else if (Char.IsWhiteSpace(ch))
            {
                _decoders.Clear();
                _decoders.Push(DecodeAttributeName);
                _decoders.Push(DecodeSeparator);

                _attributes.Last().Value = _chars.ToString();
                _chars.Clear();
            }
            else
            {
                _chars.Append(ch);
            }
        }

        private void DecodeAttributeName(char ch)
        {
            if (ch == '/')
            {
                IsSelfClosing = true;
            }
            else if (Char.IsWhiteSpace(ch))
            {
                _decoders.Clear();
                _decoders.Push(DecodeAttributeValue);
                _decoders.Push(DecodeSeparator);
                _attributes.Add(new TagAttribute {Name = _chars.ToString()});
                _chars.Clear();
            }
            else if (ch == '=')
            {
                _decoders.Clear();
                _decoders.Push(DecodeAttributeValue);
                _attributes.Add(new TagAttribute {Name = _chars.ToString()});
                _chars.Clear();
            }
            else
            {
                _chars.Append(ch);
            }
        }

        private void DecodeSeparator(char ch)
        {
            if (Char.IsWhiteSpace(ch))
                // ignore whitespace
                return;

            _decoders.Pop();
            _decoders.Peek()(ch);
        }

        private void DecodeConstantString(char ch)
        {
            if (ch == '"')
            {
                _decoders.Clear();
                _decoders.Push(DecodeAttributeName);
                _decoders.Push(DecodeSeparator);

                _attributes.Last().Value = _chars.ToString();
                _chars.Clear();
            }
            else
            {
                _chars.Append(ch);
            }
        }

        public void Render(TextWriter writer)
        {
            writer.Write("<");
            if (IsClosingTag)
            {
                writer.Write("/");
            }
            writer.Write(_name.ToString());
            foreach (var attr in _attributes)
            {
                writer.Write(" ");
                writer.Write(attr.Name);
                writer.Write("=\"");
                writer.Write(attr.Value);
                writer.Write("\"");
            }
            //if (_chars.Length > 0)
            //{
            //    writer.Write(" ");
            //    writer.Write(_chars.ToString().TrimEnd(' ', '\n', '\r', '\t'));
            //}
            if (IsSelfClosing)
                writer.Write(" /");

            writer.Write(">");
        }

        public string TagName
        {
            get { return _name.ToString(); }
        }

        public IDictionary<string, string> Attributes
        {
            get
            {
                return _attributes.ToDictionary(attr => attr.Name, attr => attr.Value);
            }
        }

    }
}