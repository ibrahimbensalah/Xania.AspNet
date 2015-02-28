using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.ModelBinding;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace Xania.AspNet.TagHelpers
{
    public class TagHelperContainer: ITagHelperContainer
    {
        private readonly Func<Type, object> _objectFactory;
        private readonly Dictionary<string, Type> _tagHelperTypes;

        public TagHelperContainer()
            : this(Activator.CreateInstance)
        {
        }

        public TagHelperContainer(Func<Type, object> objectFactory)
        {
            _objectFactory = objectFactory;
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
                var tagHelper = (ITagHelper)_objectFactory(tagHelperType);
                if (tagHelper == null)
                    return null;

                Bind(tagHelper, tagHelperType, attributes);

                return tagHelper;
            }
            return null;
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
