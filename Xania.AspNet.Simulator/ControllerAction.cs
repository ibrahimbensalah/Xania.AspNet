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
            var controllerContext = _actionRequest.CreateContext(Controller, ActionDescriptor, ValueProvider);

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
    }
}
