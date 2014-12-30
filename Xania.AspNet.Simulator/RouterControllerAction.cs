using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public class RouterAction : ControllerAction
    {
        private readonly Router _router;

        public RouterAction(Router router)
        {
            _router = router;
        }

        public override ControllerActionResult Execute()
        {
            ControllerBase controller;
            var actionDescriptor = GetActionDescriptor(out controller);

            if (actionDescriptor == null)
                return null;

            return Execute(controller.ControllerContext, actionDescriptor);
        }

        protected virtual ActionDescriptor GetActionDescriptor(out ControllerBase controller)
        {
            controller = null;
            var context = AspNetUtility.GetContext(this);
            var routeData = _router.Routes.GetRouteData(context);

            if (routeData == null)
                return null;

            var controllerName = routeData.GetRequiredString("controller");
            controller = _router.CreateController(controllerName);
            var controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());

            var actionName = routeData.GetRequiredString("action");
            var httpContext = AspNetUtility.GetContext(String.Format("/{0}/{1}", controllerName, actionName), HttpMethod, User ?? CreateAnonymousUser());

            var requestContext = new RequestContext(httpContext, routeData);

            controller.ControllerContext = new ControllerContext(requestContext, controller);

            return controllerDescriptor.FindAction(controller.ControllerContext, actionName);
        }
    }
}