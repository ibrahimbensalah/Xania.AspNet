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

        public void WriteContent(TextWriter writer, char ch)
        {
            writer.Write(Char.ToUpper(ch));
        }

        public void RenderAfterContent(TextWriter writer)
        {
            writer.Write("</span></h1></div>");
        }

        public void RenderBeforeContent(TextWriter writer)
        {
            writer.Write("<div><h1>");
            writer.Write(Attributes["P1"]);
            writer.Write("<span>");
        }
    }
}