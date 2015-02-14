using System;
using System.Collections.Generic;
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

        public HtmlDecoder(TextWriter writer, ITagHelperProvider tagHelperProvider)
        {
            _writer = writer;
            _tagHelperProvider = tagHelperProvider;
            _tagDecoders = new Stack<ITagDecoder>();
            _tagHelpers = new Stack<ITagHelper>();
            _tagHelperDictionary = new Dictionary<ITagDecoder, ITagHelper>();
        }

        public void Append(char ch)
        {
            switch (ch)
            {
                case '<':
                    _tagDecoders.Push(new TagDecoder());
                    break;
                case '>':
                    CloseDecoder();
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
                _tagHelpers.Peek().WriteContent(_writer, ch);
            }
            else 
            {
                _writer.Write(ch);
            }
        }

        public void CloseDecoder()
        {
            var current = _tagDecoders.Peek();
            current.Closed = true;

            var tagHelper = GetTagHelper(current);
            tagHelper.RenderContext = new RenderContext
            {
                TagName = current.TagName,
                Values = current.Values
            };

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
            tagHelper = _tagHelperProvider.GetTagHelper(tagDecoder.TagName);

            if (tagHelper != null)
            {
                _tagHelperDictionary.Add(tagDecoder, tagHelper);
                return tagHelper;
            }
            return new TagHelperAdapter(tagDecoder);
        }
        
        private void UpdateTagHelpers(ITagDecoder tagDecoder)
        {
            if (tagDecoder.IsClosingTag)
            {
            }
            else if (tagDecoder.IsSelfClosing)
            {
            }
            else
            {
                var tagHelper = _tagHelperProvider.GetTagHelper(tagDecoder.TagName);
            }
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

    public interface ITagHelperProvider
    {
        ITagHelper GetTagHelper(string tagName);
    }
}
