using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class ActionRequest : IActionRequest
    {
        public ActionRequest()
        {
        }

        public ActionRequest(string url, string method)
        {
            if (url.StartsWith("~"))
                url = url.Substring(1);

            UriPath = url;
            HttpMethod = method;
            HttpVersion = "HTTP/1.1";
        }

        public IPrincipal User { get; set; }

        public IValueProvider ValueProvider { get; set; }

        public ControllerBase Controller { get; set; }

        public ActionDescriptor ActionDescriptor { get; set; }

        public string HttpMethod { get; set; }

        public string UriPath { get; set; }

        public string HttpVersion { get; set; }

        public virtual ControllerContext CreateContext()
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
        public static ActionRequest Parse(String raw)
        {
            var lines = raw.Split('\n');
            var first = lines.First();

            var parts = first.Split(' ');
            var httpMethod = parts[0];
            var uriPath = parts[1];

            var httpVersion = parts[2];

            return new ActionRequest
            {
                UriPath = uriPath,
                HttpMethod = httpMethod,
                HttpVersion = httpVersion
            };
        }
    }
}