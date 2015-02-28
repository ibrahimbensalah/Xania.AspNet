using System;
using System.IO;
using System.Security.Policy;
using System.Web.Mvc;

namespace Xania.AspNet.TagHelpers
{
    public class AnchorTagHelper: TagHelperBase
    {
        private readonly UrlHelper _url;

        public AnchorTagHelper(UrlHelper url)
        {
            _url = url;
        }

        public String Action { get; set; }

        public String Controller { get; set; }

        public override void RenderBeforeContent(TextWriter writer)
        {
            writer.Write("<a");
            if (!Attributes.ContainsKey("href"))
            {
                writer.Write(" href=\"");
                writer.Write(_url.Action(Action, Controller));
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