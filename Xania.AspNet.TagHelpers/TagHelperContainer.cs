using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Xania.AspNet.TagHelpers
{
    public class TagHelperContainer: ITagHelperContainer
    {
        private readonly Dictionary<string, Type> _tagHelperTypes;

        public TagHelperContainer()
        {
            _tagHelperTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        }

        public virtual void Register(string tagName, Type type)
        {
            _tagHelperTypes.Add(tagName, type);
        }

        public virtual ITagHelper GetTagHelper(string tagName, IDictionary<string, string> attributes)
        {
            Type tagHelperType;
            if (_tagHelperTypes.TryGetValue(tagName, out tagHelperType))
            {
                var tagHelper = GetTagHelper(tagHelperType);
                if (tagHelper == null)
                    return null;

                Bind(tagHelper, tagHelperType, attributes);

                return tagHelper;
            }
            return null;
        }

        protected virtual ITagHelper GetTagHelper(Type tagHelperType)
        {
            return (ITagHelper)Activator.CreateInstance(tagHelperType);
        }

        protected virtual void Bind(ITagHelper tagHelper, Type tagType, IDictionary<string, string> attributes)
        {
            foreach (var propertyDescr in TypeDescriptor.GetProperties(tagType).OfType<PropertyDescriptor>().Where(p => !p.IsReadOnly))
            {
                string value;
                if (attributes.TryGetValue(propertyDescr.Name, out value))
                {
                    var convertor = propertyDescr.Converter;
                    propertyDescr.SetValue(tagHelper, convertor.ConvertFrom(value));

                    attributes.Remove(propertyDescr.Name);
                }
            }
            tagHelper.Attributes = attributes;
        }

    }
}
