using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public class RouterAction : IAction
    {
        private readonly ActionRequest _actionRequest;
        private readonly Router _router;

        public RouterAction(ActionRequest actionRequest, Router router)
        {
            _actionRequest = actionRequest;
            _router = router;
        }

        public ControllerActionResult Execute()
        {
            var context = AspNetUtility.GetContext(_actionRequest);
            RouteData routeData = _router.Routes.GetRouteData(context);

            if (routeData == null)
                return null;

            var controllerName = routeData.GetRequiredString("controller");
            var controller = _router.CreateController(controllerName);
            var controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());
            var actionDescriptor = controllerDescriptor.FindAction(new ControllerContext(context, routeData, controller), routeData.GetRequiredString("action"));

            if (actionDescriptor == null)
                return null;

            var controllerContext = _actionRequest.CreateContext(_actionRequest, controller, actionDescriptor);

            return _actionRequest.Execute(controllerContext, actionDescriptor);
        }
    }
}