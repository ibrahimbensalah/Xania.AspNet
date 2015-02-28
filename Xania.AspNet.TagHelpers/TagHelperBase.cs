using System.Collections.Generic;
using System.IO;

namespace Xania.AspNet.TagHelpers
{
    public abstract class TagHelperBase: ITagHelper
    {
        public virtual IDictionary<string, string> Attributes { get; set; }

        public virtual void WriteContent(TextWriter writer, char ch)
        {
            writer.Write(ch);
        }

        public abstract void RenderAfterContent(TextWriter writer);

        public abstract void RenderBeforeContent(TextWriter writer);

        protected virtual void RenderAttributes(TextWriter writer)
        {
            foreach (var kvp in Attributes)
            {
                writer.Write(" ");
                writer.Write(kvp.Key);
                writer.Write("=\"");
                writer.Write(kvp.Value);
                writer.Write("\"");
            }
        }
    }
}