using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public class RouterAction : ControllerAction, IControllerAction
    {
        private readonly Router _router;

        public RouterAction(Router router)
        {
            _router = router;
        }

        public ControllerActionResult Execute()
        {
            var context = AspNetUtility.GetContext(this);
            var routeData = _router.Routes.GetRouteData(context);

            if (routeData == null)
                return null;

            var controllerName = routeData.GetRequiredString("controller");
            var controller = _router.CreateController(controllerName);
            var controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());
            var actionDescriptor = controllerDescriptor.FindAction(new ControllerContext(context, routeData, controller), routeData.GetRequiredString("action"));

            if (actionDescriptor == null)
                return null;

            var controllerContext = CreateContext(this, controller, actionDescriptor);

            return Execute(controllerContext, actionDescriptor);
        }
    }
}