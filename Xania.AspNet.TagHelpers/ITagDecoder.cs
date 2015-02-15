using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Xania.AspNet.TagHelpers
{
    public interface ITagDecoder
    {
        void Append(char ch);
        void Render(TextWriter writer);
        string TagName { get; }
        IDictionary<string, string> Attributes { get; }
        bool IsClosingTag { get; }
        bool IsSelfClosing { get; }
        // bool Closed { get; set; }
        bool Closed { get; set; }
    }
}