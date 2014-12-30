using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class DirectControllerAction : ControllerAction
    {
        public DirectControllerAction(ControllerBase controller, ActionDescriptor actionDescriptor)
        {
            Controller = controller;
            ActionDescriptor = actionDescriptor;
        }

        public ControllerBase Controller { get; private set; }

        public ActionDescriptor ActionDescriptor { get; private set; }

        public override ControllerActionResult Execute()
        {
            var controllerContext = CreateContext(this, Controller, ActionDescriptor);
            return Execute(controllerContext, ActionDescriptor);
        }
    }
}