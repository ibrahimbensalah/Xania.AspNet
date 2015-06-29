using System;
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
            : base(mvcApplication.Routes, mvcApplication.ViewEngines)
        {
            MvcApplication = mvcApplication;
        }

        private HttpContextBase HttpContext { get; set; }

        public IMvcApplication MvcApplication { get; set; }

        public HttpControllerAction(IMvcApplication mvcApplication, [NotNull] HttpContextBase context)
            : this(mvcApplication)
        {
            if (context == null) 
                throw new ArgumentNullException("context");

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

            var controller = MvcApplication.ControllerFactory.CreateController(HttpContext, controllerName);
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