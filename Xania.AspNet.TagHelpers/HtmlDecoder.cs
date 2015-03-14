using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xania.AspNet.TagHelpers
{
    public class HtmlDecoder
    {
        private readonly TextWriter _writer;
        private readonly ITagHelperProvider _tagHelperProvider;
        private readonly Stack<ITagDecoder> _tagDecoders;
        private readonly Dictionary<ITagDecoder, ITagHelper> _tagHelperDictionary;
        private readonly Stack<ITagHelper> _tagHelpers;
        private readonly StringBuilder _chars;
        private Action<char> _handler;

        public HtmlDecoder(TextWriter writer, ITagHelperProvider tagHelperProvider)
        {
            _writer = writer;
            _tagHelperProvider = tagHelperProvider;
            _tagDecoders = new Stack<ITagDecoder>();
            _tagHelpers = new Stack<ITagHelper>();
            _tagHelperDictionary = new Dictionary<ITagDecoder, ITagHelper>();
            _chars = new StringBuilder();
            _handler = AppendNonTag;
        }

        public void Append(char ch)
        {
            _handler(ch);
        }

        private void AppendNonTag(char ch)
        {
            switch (ch)
            {
                case '<':
                    _handler = AppendStart;
                    break;
                case '>':
                    CloseDecoder();
                    break;
                default:
                    AppendContent(ch);
                    break;
            }
        }

        private void AppendStart(char ch)
        {
            if (ch == '!')
            {
                _handler = AppendSpecial;
                _writer.Write("<!");

                //if (_chars.Length == 3)
                //{
                //    //if (_chars[1] == '-' && _chars[2] == '-')
                //    //{
                //    //    // is comment
                //    //    _handler = AppendComment;
                //    //}
                //    //else
                //    {
                //        _handler = AppendSpecial;
                //    }
                //    _chars.Clear();
                //}
            }
            else
            {
                _tagDecoders.Push(new TagDecoder());
                _handler = AppendTag;
                Append(ch);
            }
        }

        private void Flush(char ch)
        {
            for (int i = 0; i < _chars.Length; i++)
                Append(_chars[i]);
            _chars.Clear();
        }

        private void AppendSpecial(char ch)
        {
            _writer.Write(ch);
            if (ch == '>')
            {
                _chars.Clear();
                _handler = AppendNonTag;
            }
            else if (_chars.Length == 0)
            {
                _chars.Append(ch);
            }
            else if (_chars.Length == 1)
            {
                _chars.Append(ch);
                if (_chars[0] == '-' && _chars[1] == '-')
                {
                    _chars.Clear();
                    _handler = AppendComment;
                }
            }
        }

        private void AppendComment(char ch)
        {
            _writer.Write(ch);
            if (_chars.Length == 0 && ch == '-')
            {
                _chars.Append(ch);
            }
            else if (_chars.Length == 1 && ch == '-')
            {
                _chars.Append(ch);
            }
            else if (_chars.Length == 2)
            {
                if (ch == '>')
                {
                    _chars.Clear();
                    _handler = AppendNonTag;
                }
                else if (ch != '-')
                {
                    _chars.Clear();
                }
            }
        }

        private void AppendTag(char ch)
        {
            switch (ch)
            {
                case '>':
                    CloseDecoder();
                    _handler = AppendNonTag;
                    break;
                default:
                    AppendContent(ch);
                    break;
            }
        }


        private void AppendContent(char ch)
        {
            if (_tagDecoders.Any() && !_tagDecoders.Peek().Closed)
                _tagDecoders.Peek().Append(ch);
            else if (_tagHelpers.Any())
            {
                _tagHelpers.Peek().RenderContent(_writer, ch);
            }
            else 
            {
                _writer.Write(ch);
            }
        }

        public void CloseDecoder()
        {
            if (_tagDecoders.Count == 0)
            {
                _writer.Write('>');
                return;
            }
            var current = _tagDecoders.Peek();
            current.Closed = true;

            var tagHelper = GetTagHelper(current);

            if (current.IsClosingTag)
            {
                _tagHelpers.Pop();
                tagHelper.RenderAfterContent(_writer);
            }
            else if (current.IsSelfClosing)
            {
                tagHelper.RenderBeforeContent(_writer);
                tagHelper.RenderAfterContent(_writer);
            }
            else
            {
                _tagHelpers.Push(tagHelper);
                tagHelper.RenderBeforeContent(_writer);
            }
        }

        private ITagHelper GetTagHelper(ITagDecoder tagDecoder)
        {
            ITagHelper tagHelper;
            if (_tagHelperDictionary.TryGetValue(tagDecoder, out tagHelper))
            {
                return tagHelper;
            }
            tagHelper = _tagHelperProvider.GetTagHelper(tagDecoder.TagName, tagDecoder.Attributes);

            if (tagHelper != null)
            {
                _tagHelperDictionary.Add(tagDecoder, tagHelper);
                return tagHelper;
            }
            return new TagHelperAdapter(tagDecoder);
        }

        public void Flush()
        {
            while (_tagDecoders.Any())
            {
                var tagDecoder = _tagDecoders.Pop();
                // Flush(tagDecoder);
            }
            _writer.Flush();
        }
    }
}
