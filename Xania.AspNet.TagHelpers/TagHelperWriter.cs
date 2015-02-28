using System.IO;
using System.Text;

namespace Xania.AspNet.TagHelpers
{
    internal class TagHelperWriter : TextWriter
    {
        private readonly TextWriter _output;
        private readonly HtmlProcessor _processor;

        public TagHelperWriter(TextWriter output, ITagHelperProvider tagHelperProvider)
            : base(output.FormatProvider)
        {
            _output = output;
            _processor = new HtmlProcessor(output, tagHelperProvider);
        }

        public override Encoding Encoding
        {
            get { return _output.Encoding; }
        }

        protected override void Dispose(bool disposing)
        {
            _output.Dispose();
        }

        public override void Close()
        {
            _output.Close();
        }

        public override void Flush()
        {
            _output.Flush();
        }

        public override void Write(char value)
        {
            _processor.Write(value);
        }
    }
}