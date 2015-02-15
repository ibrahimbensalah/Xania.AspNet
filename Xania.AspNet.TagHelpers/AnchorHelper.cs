using System;
using System.Collections.Generic;
using System.IO;

namespace Xania.AspNet.TagHelpers
{
    public class AnchorHelper: ITagHelper
    {
        public string TagName { get; set; }

        public IDictionary<string, string> Attributes { get; set; }

        public void WriteContent(TextWriter writer, char ch)
        {
            writer.Write(ch);
        }

        public void RenderAfterContent(TextWriter writer)
        {
            writer.Write("</a>");
        }

        public void RenderBeforeContent(TextWriter writer)
        {
            writer.Write("<a");
            if (!Attributes.ContainsKey("href") && Attributes.ContainsKey("action") && Attributes.ContainsKey("controller"))
            {
                var action = PopAttribute("action");
                var controller = PopAttribute("controller");

                writer.Write(" href=\"");
                writer.Write(String.Format("/{0}/{1}", controller, action).ToLowerInvariant());
                writer.Write("\"");
            }
            RenderAttributes(writer);
            writer.Write(">");
        }

        private void RenderAttributes(TextWriter writer)
        {
            foreach (var kvp in this.Attributes)
            {
                writer.Write(" ");
                writer.Write(kvp.Key);
                writer.Write("=\"");
                writer.Write(kvp.Value);
                writer.Write("\"");
            }
        }

        private string PopAttribute(string name)
        {
            string value;
            if (Attributes.TryGetValue(name, out value))
            {
                Attributes.Remove(name);
                return value;
            }
            return null;
        }
    }
}