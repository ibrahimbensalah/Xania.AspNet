using System;
using System.Collections.Generic;
using System.IO;

namespace Xania.AspNet.TagHelpers.Tests
{
    public class TagC: ITagHelper
    {
        public IDictionary<string, TagAttribute> Attributes { get; set; }

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
            writer.Write("<div><h1>");
            TagAttribute p1;
            if (Attributes.TryGetValue("P1", out p1))
                writer.Write(p1.RawValue);
            writer.Write("<span>");
        }
    }


    public class TagWithNs : TagHelperBase
    {
        public override void RenderBeforeContent(TextWriter writer)
        {
            writer.Write("<div>xn");
        }

        public override void RenderAfterContent(TextWriter writer)
        {
            writer.Write("</div>");
        }
    }
}