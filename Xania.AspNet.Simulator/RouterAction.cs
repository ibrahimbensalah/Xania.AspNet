using System;
using System.Web;
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

        public override ControllerActionResult Execute(HttpContextBase httpContext)
        {
            var actionContext = GetActionContext(httpContext);
            var actionDescriptor = actionContext.ActionDescriptor;

            if (actionDescriptor == null)
                return null;

            return Execute(actionContext.ControllerContext, actionDescriptor);
        }

        public override ActionContext GetActionContext(HttpContextBase httpContext1)
        {
            var context = AspNetUtility.GetContext(this);
            var routeData = _router.GetRouteData(context);

            if (routeData == null)
                return null;

            var controllerName = routeData.GetRequiredString("controller");
            var controller = _router.CreateController(controllerName);
            var controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());

            var actionName = routeData.GetRequiredString("action");
            var httpContext = AspNetUtility.GetContext(String.Format("/{0}/{1}", controllerName, actionName), HttpMethod, User ?? CreateAnonymousUser());

            var requestContext = new RequestContext(httpContext, routeData);

            controller.ControllerContext = new ControllerContext(requestContext, controller);

            return new ActionContext
            {
                ControllerContext = controller.ControllerContext,
                ActionDescriptor = controllerDescriptor.FindAction(controller.ControllerContext, actionName)
            };
        }

        public override HttpContextBase CreateHttpContext()
        {
            return AspNetUtility.GetContext(this);
        }
    }
}