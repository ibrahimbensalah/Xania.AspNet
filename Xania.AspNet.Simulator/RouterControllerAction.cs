using System.Web.Mvc;

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
            ControllerBase controller;
            var actionDescriptor = GetActionDescriptor(out controller);

            if (actionDescriptor == null)
                return null;

            var controllerContext = CreateContext(this, controller, actionDescriptor);

            return Execute(controllerContext, actionDescriptor);
        }

        private ActionDescriptor GetActionDescriptor(out ControllerBase controller)
        {
            controller = null;
            var context = AspNetUtility.GetContext(this);
            var routeData = _router.Routes.GetRouteData(context);

            if (routeData == null)
                return null;

            var controllerName = routeData.GetRequiredString("controller");
            controller = _router.CreateController(controllerName);
            var controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());
            var actionDescriptor = controllerDescriptor.FindAction(new ControllerContext(context, routeData, controller),
                routeData.GetRequiredString("action"));
            return actionDescriptor;
        }
    }
}