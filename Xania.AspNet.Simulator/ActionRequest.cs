using System;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class ActionRequest : IActionRequest
    {
        public IPrincipal User { get; set; }

        public virtual IValueProvider ValueProvider { get; set; }

        public ControllerBase Controller { get; set; }

        public ControllerContext CreateContext(ActionDescriptor ActionDescriptor)
        {
            var controllerDescriptor = ActionDescriptor.ControllerDescriptor;
            var controllerName = controllerDescriptor.ControllerName;

            var requestContext = AspNetUtility.CreateRequestContext(ActionDescriptor.ActionName, controllerName,
               HttpMethod, User ?? CreateAnonymousUser());

            var controllerContext = new ControllerContext(requestContext, Controller);
            Controller.ControllerContext = controllerContext;
            // Use empty value provider by default to prevent use of ASP.NET MVC default value providers
            // Its not the purpose of this simulator framework to validate the ASP.NET MVC default value 
            // providers. Either a value provider is not need in case model values are predefined or a 
            // custom implementation is provided.
            Controller.ValueProvider = ValueProvider ?? new ValueProviderCollection();
            return controllerContext;
        }

        protected virtual IPrincipal CreateAnonymousUser()
        {
            return new GenericPrincipal(new GenericIdentity(String.Empty), new string[] { });
        }

        public string HttpMethod { get; set; }
    }
}