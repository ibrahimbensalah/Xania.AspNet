using System.IO;
using System.Text;

namespace Xania.AspNet.TagHelpers
{
    internal class TagHelperWriter : TextWriter
    {
        private readonly TextWriter _output;
        private readonly HtmlProcessor _processor;

        public TagHelperWriter(TextWriter output, TagHelperProvider tagHelperProvider)
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
            var bytes = Encoding.GetBytes(new[] { value });
            _processor.Write(bytes, 0, bytes.Length);
        }
    }
}