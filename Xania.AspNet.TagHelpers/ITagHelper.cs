using System;
using System.Collections.Generic;
using System.IO;

namespace Xania.AspNet.TagHelpers
{
    public interface ITagHelper
    {
        IDictionary<string, string> Attributes { get; set; }
        void RenderContent(TextWriter writer, char ch);
        void RenderAfterContent(TextWriter writer);
        void RenderBeforeContent(TextWriter writer);
    }
}

