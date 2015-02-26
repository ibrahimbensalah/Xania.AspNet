using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xania.AspNet.TagHelpers
{
    public class TagHelperProvider: ITagHelperProvider
    {
        private readonly Func<Type, object> _objectFactory;
        private readonly Dictionary<string, Type> _tagHelperTypes;

        public TagHelperProvider()
            : this(Activator.CreateInstance)
        {
        }
        public TagHelperProvider(Func<Type, object> objectFactory)
        {
            _objectFactory = objectFactory;
            _tagHelperTypes = new Dictionary<string, Type>();
        }

        public void Register<T>(string tagName)
            where T : ITagHelper
        {
            _tagHelperTypes.Add(tagName, typeof(T));
        }

        public virtual ITagHelper GetTagHelper(string tagName)
        {
            Type tagHelperType;
            if (_tagHelperTypes.TryGetValue(tagName, out tagHelperType))
            {
                return (ITagHelper)_objectFactory(tagHelperType);
            }
            return null;
        }
    }
}
