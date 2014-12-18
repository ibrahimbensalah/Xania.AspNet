using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    internal class ControllerAction: IAction
    {
        private readonly IActionRequest _actionRequest;

        private IPrincipal _user;
        public ControllerBase Controller { get; private set; }
        public ActionDescriptor ActionDescriptor { get; private set; }

        public IValueProvider ValueProvider { get; set; }

        public FilterProviderCollection FilterProviders { get; private set; }

        public ControllerAction(ControllerBase controller, ActionDescriptor actionDescriptor, IActionRequest actionRequest)
        {
            _actionRequest = actionRequest;
            if (controller == null)
                throw new ArgumentNullException("controller");

            if (actionDescriptor == null) 
                throw new ArgumentNullException("actionDescriptor");

            Controller = controller;
            ActionDescriptor = actionDescriptor;
            FilterProviders = new FilterProviderCollection(System.Web.Mvc.FilterProviders.Providers);
        }

        public ControllerActionResult Execute()
        {
            var controllerDescriptor = ActionDescriptor.ControllerDescriptor;
            var controllerName = controllerDescriptor.ControllerName;

            var requestContext = AspNetUtility.CreateRequestContext(ActionDescriptor.ActionName, controllerName, 
                _actionRequest.HttpMethod, _actionRequest.User ?? AnonymousUser);
            
            var controllerContext = new ControllerContext(requestContext, Controller);
            Controller.ControllerContext = controllerContext;
            // Use empty value provider by default to prevent use of ASP.NET MVC default value providers
            // Its not the purpose of this simulator framework to validate the ASP.NET MVC default value 
            // providers. Either a value provider is not need in case model values are predefined or a 
            // custom implementation is provided.
            Controller.ValueProvider = ValueProvider ?? new ValueProviderCollection();

            if (ActionDescriptor.GetSelectors().Any(selector => !selector.Invoke(controllerContext)))
            {
                throw new InvalidOperationException(String.Format("Http method '{0}' is not allowed", _actionRequest.HttpMethod));
            }

            var filters = FilterProviders.GetFilters(controllerContext, ActionDescriptor);

            var invoker = new SimpleActionInvoker(controllerContext, ActionDescriptor, filters);

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
