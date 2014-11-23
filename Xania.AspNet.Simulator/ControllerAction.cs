using System;
using System.IO;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class ControllerAction
    {
        private IPrincipal _user;
        private readonly ControllerBase _controller;
        private readonly ActionDescriptor _actionDescriptor;

        public ControllerAction(ControllerBase controller, ActionDescriptor actionDescriptor)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");

            if (actionDescriptor == null) 
                throw new ArgumentNullException("actionDescriptor");

            _controller = controller;
            _actionDescriptor = actionDescriptor;
        }

        public void Authenticate(IPrincipal user)
        {
            if (user == null) 
                throw new ArgumentNullException("user");

            _user = user;
        }

        public ControllerActionResult Execute()
        {
            var controllerDescriptor = new ReflectedControllerDescriptor(_controller.GetType());
            var controllerName = controllerDescriptor.ControllerName;

            var requestContext = AspNetUtility.CreateRequestContext(_actionDescriptor.ActionName, controllerName, _user ?? AnonymousUser, new MemoryStream());

            var controllerContext = new ControllerContext(requestContext, _controller);

            var invoker = new MvcActionInvoker(controllerContext, _actionDescriptor);

            return new ControllerActionResult
            {
                ControllerContext = controllerContext,
                ActionResult = invoker.InvokeAction()
            };
        }

        static ControllerAction()
        {
            AnonymousUser = new GenericPrincipal(new GenericIdentity(String.Empty), new string[] {});
        }

        public static IPrincipal AnonymousUser { get; private set; }

    }
}
