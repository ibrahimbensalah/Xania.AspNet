using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public interface IControllerAction
    {
        ControllerBase Controller { get; }

        void Authenticate(IPrincipal user);

        ControllerActionResult Execute();

        FilterProviderCollection FilterProviders { get; }
    }

    public class ControllerAction: IControllerAction
    {
        private readonly string _httpMethod;
        private IPrincipal _user;
        public ControllerBase Controller { get; private set; }
        public ActionDescriptor ActionDescriptor { get; private set; }

        public IValueProvider ValueProvider { get; set; }

        public FilterProviderCollection FilterProviders { get; private set; }

        public ControllerAction(ControllerBase controller, ActionDescriptor actionDescriptor, string httpMethod = "GET")
        {
            _httpMethod = httpMethod;
            if (controller == null)
                throw new ArgumentNullException("controller");

            if (actionDescriptor == null) 
                throw new ArgumentNullException("actionDescriptor");

            Controller = controller;
            ActionDescriptor = actionDescriptor;
            FilterProviders = new FilterProviderCollection(System.Web.Mvc.FilterProviders.Providers);
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

            var requestContext = AspNetUtility.CreateRequestContext(ActionDescriptor.ActionName, controllerName, _httpMethod, _user ?? AnonymousUser);
            
            var controllerContext = new ControllerContext(requestContext, Controller);
            Controller.ControllerContext = controllerContext;
            // Use empty value provider by default to prevent use of ASP.NET MVC default value providers
            // Its not the purpose of this simulator framework to validate the ASP.NET MVC default value 
            // providers. Either a value provider is not need in case model values are predefined or a 
            // custom implementation is provided.
            Controller.ValueProvider = ValueProvider ?? new ValueProviderCollection();

            if (ActionDescriptor.GetSelectors().Any(selector => !selector.Invoke(controllerContext)))
            {
                throw new InvalidOperationException(String.Format("Http method '{0}' is not allowed", _httpMethod));
            }

            var filters = FilterProviders.GetFilters(controllerContext, ActionDescriptor);

            var invoker = new MvcActionInvoker(controllerContext, ActionDescriptor, filters);

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
