using System;
using System.Collections.Generic;
using System.IO;

namespace Xania.AspNet.TagHelpers
{
    public interface ITagHelper
    {
        string TagName { get; set; }
        IDictionary<string, string> Attributes { get; set; }
        void WriteContent(TextWriter writer, char ch);
        void RenderAfterContent(TextWriter writer);
        void RenderBeforeContent(TextWriter writer);
    }
}

