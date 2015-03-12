using System;
using System.Collections.Generic;
using System.IO;
using Xania.AspNet.TagHelpers.Tests.Annotations;

namespace Xania.AspNet.TagHelpers.Tests
{
    [UsedImplicitly]
    public class TagC: ITagHelper
    {
        public IDictionary<string, string> Attributes { get; set; }

        public void RenderContent(TextWriter writer, char ch)
        {
            writer.Write(Char.ToUpper(ch));
        }

        public void RenderAfterContent(TextWriter writer)
        {
            writer.Write("</span></h1></div>");
        }

        public void RenderBeforeContent(TextWriter writer)
        {
            string p1;
            Attributes.TryGetValue("P1", out p1);
            writer.Write("<div><h1>");
            writer.Write(p1 ?? string.Empty);
            writer.Write("<span>");
        }
    }
}