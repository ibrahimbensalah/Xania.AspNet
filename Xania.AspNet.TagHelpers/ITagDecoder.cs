using System.Collections.Generic;
using System.IO;

namespace Xania.AspNet.TagHelpers
{
    public interface ITagDecoder
    {
        void Append(char ch);
        void Render(TextWriter writer);
        string TagName { get; }
        IEnumerable<TagAttribute> Attributes { get; }
        bool IsClosingTag { get; }
        bool IsSelfClosing { get; }
        // bool Closed { get; set; }
        bool Closed { get; set; }
    }
}