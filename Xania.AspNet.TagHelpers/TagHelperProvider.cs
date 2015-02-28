using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.ModelBinding;

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

        public virtual ITagHelper GetTagHelper(string tagName, IDictionary<string, string> attributes)
        {
            Type tagHelperType;
            if (_tagHelperTypes.TryGetValue(tagName, out tagHelperType))
            {
                var instance = (ITagHelper)Activator.CreateInstance(tagHelperType);
                Bind(instance, new Dictionary<string, string>(attributes, StringComparer.OrdinalIgnoreCase));
                return instance;
            }
            return null;
        }

        private void Bind(ITagHelper tagHelper, IDictionary<string, string> attributes)
        {
            foreach (var propertyInfo in tagHelper.GetType().GetProperties().Where(p => p.CanWrite))
            {
                string value;
                if (attributes.TryGetValue(propertyInfo.Name, out value))
                {
                    var convertor = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
                    propertyInfo.SetValue(tagHelper, convertor.ConvertFrom(value));

                    attributes.Remove(propertyInfo.Name);
                }
            }
            tagHelper.Attributes = attributes;
        }

    }
}
