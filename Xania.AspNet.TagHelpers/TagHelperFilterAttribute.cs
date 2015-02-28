using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.TagHelpers
{
    public class TagHelperFilterAttribute : ActionFilterAttribute
    {
        private readonly IDependencyResolver _defaultDependencyResolver;

        public TagHelperFilterAttribute()
            : this(DependencyResolver.Current)
        {
        }

        private TagHelperFilterAttribute(IDependencyResolver defaultDependencyResolver)
        {
            _defaultDependencyResolver = defaultDependencyResolver;
        }


        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.Result = GetFilterResult(filterContext);
        }

        protected virtual ActionResult GetFilterResult(ActionExecutedContext filterContext)
        {
            return new TagHelperFilterResult(_defaultDependencyResolver, filterContext.Result, GetCustomTagHelpers());
        }

        protected virtual IEnumerable<KeyValuePair<string, Type>> GetCustomTagHelpers()
        {
            return Enumerable.Empty<KeyValuePair<string, Type>>();
        }
    }
}