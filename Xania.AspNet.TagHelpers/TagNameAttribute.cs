using System;

namespace Xania.AspNet.TagHelpers
{
    public class TagNameAttribute: Attribute
    {
        public TagNameAttribute(params String[] names)
        {
            Names = names;
        }

        public virtual string[] Names { get; private set; }
    }
}