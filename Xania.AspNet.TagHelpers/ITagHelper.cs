using System;
using System.IO;

namespace Xania.AspNet.TagHelpers
{
    public interface ITagHelper
    {
        IRenderContext RenderContext { get; set; }
        void WriteContent(TextWriter writer, char ch);
        void RenderAfterContent(TextWriter writer);
        void RenderBeforeContent(TextWriter writer);
    }

    public interface IRenderContext
    {
        String TagName { get; }

        object GetValue(string name);
    }
}

