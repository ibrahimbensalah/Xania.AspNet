using System;
using System.ComponentModel;
using System.IO;

namespace Xania.AspNet.TagHelpers.Tests
{
    public class TestTagHelper: TagHelperBase
    {
        public string Controller { get; set; }

        public string Action { get; set; }

        public override void RenderBeforeContent(TextWriter writer)
        {
            writer.Write("<a href=\"");
            writer.Write(String.Format("/{0}/{1}", Controller, Action).ToLowerInvariant());
            writer.Write("\"");
            RenderAttributes(writer);
            writer.Write(">");
        }

        public override void RenderAfterContent(TextWriter writer)
        {
            writer.Write("</a>");
        }

    }
}