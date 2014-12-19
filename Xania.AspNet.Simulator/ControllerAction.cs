using System;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    internal class ControllerAction: IAction
    {
        private readonly IActionRequest _actionRequest;
        private readonly ControllerBase _controller;
        private readonly ActionDescriptor _actionDescriptor;

        public ControllerAction(IActionRequest actionRequest, ControllerBase controller, ActionDescriptor actionDescriptor)
        {
            _actionRequest = actionRequest;
            _controller = controller;
            _actionDescriptor = actionDescriptor;
        }

        public ControllerActionResult Execute()
        {
            var controllerContext = CreateContext();
            var actionDescriptor = _actionRequest.ActionDescriptor;
            if (actionDescriptor.GetSelectors().Any(selector => !selector.Invoke(controllerContext)))
            {
                throw new InvalidOperationException(String.Format("Http method '{0}' is not allowed", _actionRequest.HttpMethod));
            }

            var filters = _actionRequest.FilterProviders.GetFilters(controllerContext, actionDescriptor);
            var invoker = new SimpleActionInvoker(controllerContext, actionDescriptor, filters);
            return new ControllerActionResult
            {
                ControllerContext = controllerContext,
                ActionResult = invoker.InvokeAction()
            };
        }

        public virtual ControllerContext CreateContext()
        {
            var controllerDescriptor = _actionDescriptor.ControllerDescriptor;
            var controllerName = controllerDescriptor.ControllerName;

            var requestContext = AspNetUtility.CreateRequestContext(_actionDescriptor.ActionName, controllerName,
               _actionRequest.HttpMethod, _actionRequest.User ?? CreateAnonymousUser());

            var controllerContext = new ControllerContext(requestContext, _controller);
            _controller.ControllerContext = controllerContext;
            // Use empty value provider by default to prevent use of ASP.NET MVC default value providers
            // Its not the purpose of this simulator framework to validate the ASP.NET MVC default value 
            // providers. Either a value provider is not need in case model values are predefined or a 
            // custom implementation is provided.
            _controller.ValueProvider = _actionRequest.ValueProvider ?? new ValueProviderCollection();
            return controllerContext;
        }

        protected virtual IPrincipal CreateAnonymousUser()
        {
            return new GenericPrincipal(new GenericIdentity(String.Empty), new string[] { });
        }


    }
}
