using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xania.AspNet.TagHelpers
{
    public class HtmlProcessor : ITagHelperProvider
    {
        private readonly Decoder _decoder;
        private readonly Object _writeLock = new object();
        private readonly TextWriter _writer;
        private readonly Dictionary<string, Type> _tagHelperTypes;
        private readonly HtmlDecoder _htmlDecoder;

        public HtmlProcessor(TextWriter writer)
        {
            _decoder = writer.Encoding.GetDecoder();
            _writer = writer;
            _tagHelperTypes = new Dictionary<string, Type>();
            _htmlDecoder = new HtmlDecoder(writer, this);
        }

        public HtmlProcessor Register<T>(string tagName)
            where T : ITagHelper
        {
            _tagHelperTypes.Add(tagName, typeof (T));
            return this;
        }

        public HtmlProcessor Write(string str)
        {
            return Write(_writer.Encoding.GetBytes(str));
        }

        public HtmlProcessor Write(byte[] bytes)
        {
            Write(bytes, 0, bytes.Length);
            return this;
        }

        public void Write(byte[] bytes, int offset, int count)
        {
            lock (_writeLock)
            {
                var chars = new char[_decoder.GetCharCount(bytes, offset, count)];
                int charCount = _decoder.GetChars(bytes, 0, bytes.Length, chars, 0);

                for (var i = 0; i < charCount; i++)
                {
                    var ch = chars[i];
                    _htmlDecoder.Append(ch);
                }
            }
        }


        //private IEnumerable<ITagDecoder> FlushStack(ITagDecoder current)
        //{
        //    while (_tagDecoders.Any())
        //    {
        //        var subject = _tagDecoders.Pop();
        //        yield return subject;

        //        if (IsOpeningTag(subject, current))
        //            break;
        //    }
        //}

        private static bool IsOpeningTag(ITagDecoder tagInSubject, ITagDecoder endTag)
        {
            return !tagInSubject.IsSelfClosing && !tagInSubject.IsClosingTag &&
                   String.Equals(tagInSubject.TagName, endTag.TagName, StringComparison.OrdinalIgnoreCase);
        }

        //public void Flush2(ITagDecoder tagDecoder)
        //{
        //    Type tagHelperType;
        //    if (_tagHelpers.TryGetValue(tagDecoder.TagName, out tagHelperType))
        //    {
        //        var tagHelper = (ITagHelper)Activator.CreateInstance(tagHelperType);
        //        var renderContext = new RenderContext
        //        {
        //            TagName = tagDecoder.TagName, 
        //            Values = tagDecoder.Values
        //        };
        //        tagHelper.Render(_writer);
        //    }
        //    else
        //    {
        //        tagDecoder.Render(_writer);
        //    }
        //}

        public virtual ITagHelper GetTagHelper(string tagName)
        {
            Type tagHelperType;
            if (_tagHelperTypes.TryGetValue(tagName, out tagHelperType))
            {
                return (ITagHelper)Activator.CreateInstance(tagHelperType);
            }
            return null;
        }


        public void Flush()
        {
            _htmlDecoder.Flush();
        }
    }

    internal class TagHelperAdapter : ITagHelper
    {
        private readonly ITagDecoder _tagDecoder;

        public TagHelperAdapter(ITagDecoder tagDecoder)
        {
            _tagDecoder = tagDecoder;
        }

        public String TagName { get; set; }

        public IDictionary<string, string> Attributes { get; set; }

        public void WriteContent(TextWriter writer, char ch)
        {
            writer.Write(ch);
        }

        public void RenderAfterContent(TextWriter writer)
        {
            if (_tagDecoder.IsClosingTag)
                _tagDecoder.Render(writer);
        }

        public void RenderBeforeContent(TextWriter writer)
        {
            if (!_tagDecoder.IsClosingTag)
                _tagDecoder.Render(writer);
        }
    }
}