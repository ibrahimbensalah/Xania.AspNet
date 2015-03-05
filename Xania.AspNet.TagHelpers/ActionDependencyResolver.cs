using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.TagHelpers
{
    public class ActionDependencyResolver : IDependencyResolver
    {
        private readonly IDependencyResolver _defaultDependencyResolver;
        private readonly ControllerContext _controllerContext;

        public ActionDependencyResolver(IDependencyResolver defaultDependencyResolver, ControllerContext controllerContext)
        {
            _defaultDependencyResolver = defaultDependencyResolver;
            _controllerContext = controllerContext;
        }

        public object GetService(Type serviceType)
        {
            if (typeof (ControllerContext) == serviceType)
                return _controllerContext;

            if (typeof (UrlHelper) == serviceType)
                return new UrlHelper(_controllerContext.RequestContext);

            if (typeof (RequestContext) == serviceType)
                return _controllerContext.RequestContext;

            return _defaultDependencyResolver.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}