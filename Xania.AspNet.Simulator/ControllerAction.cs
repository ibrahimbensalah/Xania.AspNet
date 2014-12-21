using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class ControllerAction : ActionRequest, IAction
    {
        private readonly ControllerBase _controller;
        private readonly ActionDescriptor _actionDescriptor;

        public ControllerAction(ControllerBase controller, ActionDescriptor actionDescriptor)
        {
            _controller = controller;
            _actionDescriptor = actionDescriptor;
        }

        public ControllerBase Controller
        {
            get { return _controller; }
        }

        public ActionDescriptor ActionDescriptor
        {
            get { return _actionDescriptor; }
        }

        public ControllerActionResult Execute()
        {
            var controllerContext = CreateContext(this, _controller, _actionDescriptor);
            return Execute(controllerContext, _actionDescriptor);
        }
    }
}