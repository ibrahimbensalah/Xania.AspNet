using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    internal class ControllerAction: IAction
    {
        private readonly IActionRequest _actionRequest;

        public FilterProviderCollection FilterProviders { get; private set; }

        public ControllerAction(IActionRequest actionRequest)
        {
            _actionRequest = actionRequest;
            FilterProviders = new FilterProviderCollection(System.Web.Mvc.FilterProviders.Providers);
        }

        public ControllerActionResult Execute()
        {
            var controllerContext = _actionRequest.CreateContext();
            var actionDescriptor = _actionRequest.ActionDescriptor;

            if (actionDescriptor.GetSelectors().Any(selector => !selector.Invoke(controllerContext)))
            {
                throw new InvalidOperationException(String.Format("Http method '{0}' is not allowed", _actionRequest.HttpMethod));
            }

            var filters = FilterProviders.GetFilters(controllerContext, actionDescriptor);

            var invoker = new SimpleActionInvoker(controllerContext, actionDescriptor, filters);

            return new ControllerActionResult
            {
                ControllerContext = controllerContext,
                ActionResult = invoker.InvokeAction()
            };
        }
    }
}
