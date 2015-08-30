using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xania.AspNet.Core;

namespace Xania.AspNet.Simulator
{
    public class HttpControllerAction : ControllerAction
    {
        public HttpControllerAction(Core.IControllerFactory controllerContainer)
            : this(new MvcApplication(controllerContainer, new EmptyContentProvider()))
        {            
        }

        public HttpControllerAction(IMvcApplication mvcApplication)
            : base(mvcApplication, null)
        {
        }

        public HttpControllerAction(IMvcApplication mvcApplication, HttpContextBase context)
            : base(mvcApplication, context)
        {
        }

        public override ActionExecutionContext GetExecutionContext()
        {
            var httpContext = HttpContext ?? AspNetUtility.GetContext(UriPath, HttpMethod, User ?? AspNetUtility.CreateAnonymousUser());
            
            var routeData = GetRouteData(httpContext);

            if (routeData == null)
                return null;

            var controllerName = routeData.GetRequiredString("controller");

            var controller = MvcApplication.ControllerFactory.CreateController(HttpContext, controllerName);
            var requestContext = new RequestContext(httpContext, routeData);

            var controllerContext = new ControllerContext(requestContext, controller);

            Initialize(controllerContext);

            return new ActionExecutionContext
            {
                ControllerContext = controllerContext,
                ActionDescriptor = GetActionDescriptor(controller, routeData)
            };
        }

        private static ActionDescriptor GetActionDescriptor(ControllerBase controller, RouteData routeData)
        {
            var controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());
            var actionName = routeData.GetRequiredString("action");
            var actionDescriptor = controllerDescriptor.FindAction(controller.ControllerContext, actionName);
            return actionDescriptor;
        }
    }
}