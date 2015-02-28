using System;
using System.IO;
using System.Web.Mvc;

namespace Xania.AspNet.TagHelpers
{
    public class AnchorTagHelper: TagHelperBase
    {
        public UrlHelper Url { get; set; }

        public String Action { get; set; }

        public String Controller { get; set; }

        public override void RenderBeforeContent(TextWriter writer)
        {
            writer.Write("<a");
            if (!Attributes.ContainsKey("href"))
            {
                writer.Write(" href=\"");
                writer.Write(Url.Action(Action, Controller));
                writer.Write("\"");
            }
            RenderAttributes(writer);
            writer.Write(">");
        }

        public override void RenderAfterContent(TextWriter writer)
        {
            writer.Write("</a>");
        }
    }
}