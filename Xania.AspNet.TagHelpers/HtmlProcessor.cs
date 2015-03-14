using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xania.AspNet.TagHelpers
{
    public class HtmlProcessor : Stream
    {
        private readonly Decoder _decoder;
        private readonly Object _writeLock = new object();
        private readonly TextWriter _writer;
        private readonly HtmlDecoder _htmlDecoder;

        public HtmlProcessor(TextWriter writer, ITagHelperProvider tagHelperProvider)
        {
            _decoder = writer.Encoding.GetDecoder();
            _writer = writer;
            _htmlDecoder = new HtmlDecoder(writer, tagHelperProvider);
        }

        public override void Write(byte[] bytes, int offset, int count)
        {
            lock (_writeLock)
            {
                var chars = new char[_decoder.GetCharCount(bytes, offset, count)];
                int charCount = _decoder.GetChars(bytes, offset, count, chars, 0);

                for (var i = 0; i < charCount; i++)
                {
                    var ch = chars[i];
                    Write(ch);
                }
            }
        }

        public virtual void Write(char ch)
        {
            _htmlDecoder.Append(ch);
        }

        public virtual void Write(string str)
        {
            if (str == null)
                return;

            for(int i=0 ; i<str.Length ; i++)
                Write(str[i]);
        }

        public override void Flush()
        {
            _htmlDecoder.Flush();
        }

        public override void Close()
        {
            _writer.Close();
        }


        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return 0L; }
        }

        public override long Position { get; set; }
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

        public void RenderContent(TextWriter writer, char ch)
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

