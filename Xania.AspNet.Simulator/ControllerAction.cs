using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class ControllerAction
    {
        public ControllerAction()
        {
            FilterProviders = new FilterProviderCollection(System.Web.Mvc.FilterProviders.Providers);
        }

        public FilterProviderCollection FilterProviders { get; private set; }

        public IPrincipal User { get; set; }

        public IValueProvider ValueProvider { get; set; }

        public string HttpMethod { get; set; }

        public string UriPath { get; set; }

        protected virtual ControllerActionResult Execute(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filters = FilterProviders.GetFilters(controllerContext, actionDescriptor);
            var invoker = new SimpleActionInvoker(controllerContext, actionDescriptor, filters);
            return new ControllerActionResult
            {
                ControllerContext = controllerContext,
                ActionResult = invoker.InvokeAction()
            };
        }

        public virtual IPrincipal CreateAnonymousUser()
        {
            return new GenericPrincipal(new GenericIdentity(String.Empty), new string[] { });
        }

        public virtual ControllerContext CreateContext(IControllerAction actionRequest, ControllerBase controller, ActionDescriptor actionDescriptor)
        {
            var controllerDescriptor = actionDescriptor.ControllerDescriptor;
            var controllerName = controllerDescriptor.ControllerName;

            var requestContext = AspNetUtility.CreateRequestContext(actionDescriptor.ActionName, controllerName,
                actionRequest.HttpMethod, actionRequest.User ?? CreateAnonymousUser());

            var controllerContext = new ControllerContext(requestContext, controller);
            controller.ControllerContext = controllerContext;
            // Use empty value provider by default to prevent use of ASP.NET MVC default value providers
            // Its not the purpose of this simulator framework to validate the ASP.NET MVC default value 
            // providers. Either a value provider is not need in case model values are predefined or a 
            // custom implementation is provided.
            controller.ValueProvider = actionRequest.ValueProvider ?? new ValueProviderCollection();

            if (actionDescriptor.GetSelectors().Any(selector => !selector.Invoke(controllerContext)))
            {
                throw new InvalidOperationException(String.Format("Http method '{0}' is not allowed", actionRequest.HttpMethod));
            }

            return controllerContext;
        }
    }
}