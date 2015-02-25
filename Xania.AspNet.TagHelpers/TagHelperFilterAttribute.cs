using System;
using System.Web.Mvc;

namespace Xania.AspNet.TagHelpers
{
    public class TagHelperFilterAttribute : ActionFilterAttribute
    {
        private readonly TagHelperProvider _tagHelperProvider;

        public TagHelperFilterAttribute()
        {
            _tagHelperProvider = new TagHelperProvider(Activator.CreateInstance);
            _tagHelperProvider.Register<AnchorHelper>("a");
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.Result = new TagHelperFilterResult(filterContext.Result as ViewResult, _tagHelperProvider);
        }
    }
}
