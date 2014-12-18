using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    internal class ControllerAction: IAction
    {
        private readonly IActionRequest _actionRequest;

        public ActionDescriptor ActionDescriptor { get; private set; }

        public IValueProvider ValueProvider { get; set; }

        public FilterProviderCollection FilterProviders { get; private set; }

        public ControllerAction(ActionDescriptor actionDescriptor, IActionRequest actionRequest)
        {
            _actionRequest = actionRequest;

            if (actionDescriptor == null) 
                throw new ArgumentNullException("actionDescriptor");

            ActionDescriptor = actionDescriptor;
            FilterProviders = new FilterProviderCollection(System.Web.Mvc.FilterProviders.Providers);
        }

        public ControllerActionResult Execute()
        {
            var controllerContext = _actionRequest.CreateContext(ActionDescriptor, ValueProvider);

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
