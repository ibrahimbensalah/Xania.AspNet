using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.TagHelpers
{
    public class TagHelperFilterAttribute : ActionFilterAttribute
    {
        private readonly IDependencyResolver _defaultDependencyResolver;
        public ICollection<Type> CustomTagHelpers { get; private set; }

        public TagHelperFilterAttribute()
            : this(DependencyResolver.Current)
        {
        }

        public TagHelperFilterAttribute(IDependencyResolver defaultDependencyResolver)
        {
            _defaultDependencyResolver = defaultDependencyResolver;
            CustomTagHelpers = new Collection<Type>();
        }


        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.Result = GetFilterResult(filterContext);
        }

        protected virtual ActionResult GetFilterResult(ActionExecutedContext filterContext)
        {
            var q = from th in CustomTagHelpers
                from name in GetTagNames(th)
                select new KeyValuePair<string, Type>(name, th);

            return new TagHelperFilterResult(_defaultDependencyResolver, filterContext.Result, q);
        }

        private IEnumerable<string> GetTagNames(Type tagHelperType)
        {
            var tagNameAttr = tagHelperType.GetCustomAttribute<TagNameAttribute>();
            if (tagNameAttr != null)
            {
                foreach (var name in tagNameAttr.Names)
                    yield return name;
            }
            else
            {
                var typeName = tagHelperType.Name;
                if (typeName.EndsWith("TagHelper"))
                    yield return typeName.Substring(0, typeName.Length - "TagHelper".Length);
                else
                    yield return typeName;
            }
        }
    }

    public class TagNameAttribute: Attribute
    {
        public TagNameAttribute(params String[] names)
        {
            Names = names;
        }

        public virtual string[] Names { get; private set; }
    }
}