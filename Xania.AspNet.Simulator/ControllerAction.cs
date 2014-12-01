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
        public ControllerBase Controller { get; private set; }
        public ActionDescriptor ActionDescriptor { get; private set; }

        public ControllerAction(ControllerBase controller, ActionDescriptor actionDescriptor)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");

            if (actionDescriptor == null) 
                throw new ArgumentNullException("actionDescriptor");

            Controller = controller;
            ActionDescriptor = actionDescriptor;
        }

        public void Authenticate(IPrincipal user)
        {
            if (user == null) 
                throw new ArgumentNullException("user");

            _user = user;
        }

        public ControllerActionResult Execute()
        {
            var controllerDescriptor = new ReflectedControllerDescriptor(Controller.GetType());
            var controllerName = controllerDescriptor.ControllerName;

            var requestContext = AspNetUtility.CreateRequestContext(ActionDescriptor.ActionName, controllerName, _user ?? AnonymousUser, new MemoryStream());

            var controllerContext = new ControllerContext(requestContext, Controller);
            Controller.ControllerContext = controllerContext;

            var invoker = new MvcActionInvoker(controllerContext, ActionDescriptor);

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
