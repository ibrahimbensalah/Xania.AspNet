using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.TagHelpers
{
    public class ActionTagHelperContainer : TagHelperContainer
    {
        private readonly IDependencyResolver _serviceResolver;
        private readonly ControllerContext _controllerContext;

        public ActionTagHelperContainer(IDependencyResolver serviceResolver, ControllerContext controllerContext)
        {
            _serviceResolver = serviceResolver;
            _controllerContext = controllerContext;
        }

        protected override ITagHelper GetTagHelper(Type tagHelperType)
        {
            var ctor = tagHelperType.GetConstructors().First();
            var args = ctor.GetParameters().Select(parameterInfo => GetService(parameterInfo.ParameterType)).ToArray();

            return (ITagHelper)ctor.Invoke(args);
        }

        private object GetService(Type serviceType)
        {
            if (typeof(ControllerContext) == serviceType)
                return _controllerContext;

            if (typeof(UrlHelper) == serviceType)
                return new UrlHelper(_controllerContext.RequestContext);

            if (typeof(RequestContext) == serviceType)
                return _controllerContext.RequestContext;

            if (typeof(ViewDataDictionary) == serviceType)
                return _controllerContext.Controller.ViewData;

            if (typeof(TempDataDictionary) == serviceType)
                return _controllerContext.Controller.TempData;

            return _serviceResolver.GetService(serviceType);

        }
    }
}