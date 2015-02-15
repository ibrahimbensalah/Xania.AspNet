using System;
using System.Collections.Generic;

namespace Xania.AspNet.TagHelpers
{
    public class TagHelperProvider: ITagHelperProvider
    {
        private readonly Dictionary<string, Type> _tagHelperTypes;

        public TagHelperProvider()
        {
            _tagHelperTypes = new Dictionary<string, Type>();
        }

        public TagHelperProvider Register<T>(string tagName)
            where T : ITagHelper
        {
            _tagHelperTypes.Add(tagName, typeof(T));
            return this;
        }

        public virtual ITagHelper GetTagHelper(string tagName)
        {
            Type tagHelperType;
            if (_tagHelperTypes.TryGetValue(tagName, out tagHelperType))
            {
                return (ITagHelper)Activator.CreateInstance(tagHelperType);
            }
            return null;
        }
    }
}
