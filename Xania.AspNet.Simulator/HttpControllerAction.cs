using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xania.AspNet.Core;

namespace Xania.AspNet.Simulator
{
    public class HttpControllerAction : ControllerAction
    {
        public HttpControllerAction(Core.IControllerFactory controllerFactory)
            : base(MvcApplication.GetRoutes())
        {
            ControllerFactory = controllerFactory;
        }

        private HttpContextBase HttpContext { get; set; }

        public Core.IControllerFactory ControllerFactory { get; set; }

        public HttpControllerAction(IMvcApplication mvcApplication, [NotNull] HttpContextBase context)
            : base(mvcApplication.Routes)
        {
            if (context == null) throw new ArgumentNullException("context");

            ControllerFactory = mvcApplication;
            WebPageProvider = mvcApplication;
            HttpContext = context;
        }

        public override ControllerActionResult Execute()
        {
            var actionContext = GetActionContext();
            var actionDescriptor = actionContext.ActionDescriptor;

            if (actionDescriptor == null)
                return null;

            return Execute(actionContext.ControllerContext, actionDescriptor);
        }

        public override ActionContext GetActionContext()
        {
            var httpContext = HttpContext ?? AspNetUtility.GetContext(UriPath, HttpMethod, User ?? AspNetUtility.CreateAnonymousUser());
            
            var routeData = Routes.GetRouteData(httpContext);

            if (routeData == null)
                return null;

            var controllerName = routeData.GetRequiredString("controller");

            var controller = ControllerFactory.CreateController(controllerName);
            var requestContext = new RequestContext(httpContext, routeData);

            var controllerContext = new ControllerContext(requestContext, controller);

            Initialize(controllerContext);

            return new ActionContext
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