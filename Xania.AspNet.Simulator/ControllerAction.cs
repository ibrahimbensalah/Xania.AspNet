using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public abstract class ControllerAction: IHttpRequest, IControllerAction
    {
        protected ControllerAction(RouteCollection routes)
            : this(routes, new ViewEngineCollection())
        {
        }

        protected ControllerAction(RouteCollection routes, ViewEngineCollection viewEngines)
        {
            Routes = routes;
            ViewEngines = viewEngines;

            FilterProviders = new FilterProviderCollection(System.Web.Mvc.FilterProviders.Providers);
            Cookies = new Collection<HttpCookie>();
            Session = new Dictionary<string, object>();
            Files = new Dictionary<string, Stream>();
        }

        public FilterProviderCollection FilterProviders { get; private set; }

        public IPrincipal User { get; set; }

        public IValueProvider ValueProvider { get; set; }

        public ICollection<HttpCookie> Cookies { get; private set; }
        public IDictionary<string, object> Session { get; private set; }
        public IDictionary<string, Stream> Files { get; private set; }

        public string HttpMethod { get; set; }

        public string UriPath { get; set; }

        public RouteCollection Routes { get; private set; }

        public ViewEngineCollection ViewEngines { get; private set; }

        public abstract ActionExecutionContext GetExecutionContext();

        protected virtual void Initialize(ControllerContext controllerContext)
        {
            var controller = controllerContext.Controller as Controller;

            if (controller != null)
            {
                // Use empty value provider by default to prevent use of ASP.NET MVC default value providers
                // Its not the purpose of this simulator framework to validate the ASP.NET MVC default value 
                // providers. Either a value provider is not need in case model values are predefined or a 
                // custom implementation is provided.
                var valueProviders = new ValueProviderCollection();
                if (ValueProvider != null)
                    valueProviders.Add(ValueProvider);
                valueProviders.Add(new SimulatorValueProvider(controllerContext, new CultureInfo("nl-NL")));

                controller.ValueProvider = valueProviders;
                controller.ControllerContext = controllerContext;
                controller.Url = new UrlHelper(controllerContext.RequestContext, Routes);
                controller.ViewEngineCollection = ViewEngines;
            }
        }

        public virtual ControllerContext Execute()
        {
            var actionContext = GetExecutionContext();
            var actionResult = GetActionResult(actionContext);

            actionResult.ExecuteResult(actionContext.ControllerContext);

            return actionContext.ControllerContext;
        }
        public virtual ActionResult GetActionResult()
        {
            var actionContext = GetExecutionContext();
            return GetActionResult(actionContext);
        }

        public virtual ActionResult GetActionResult(ActionExecutionContext actionExecutionContext)
        {
            var invoker = GetActionInvoker(actionExecutionContext);
            return invoker.GetActionResult();
        }

        private SimulatorActionInvoker GetActionInvoker(ActionExecutionContext actionExecutionContext)
        {
            var filters = FilterProviders.GetFilters(actionExecutionContext.ControllerContext, actionExecutionContext.ActionDescriptor);

            return new SimulatorActionInvoker(actionExecutionContext, filters, Routes);
        }

        public ActionResult Authorize()
        {
            return GetAuthorizationResult();
        }

        public virtual ActionResult GetAuthorizationResult()
        {
            return GetAuthorizationResult(GetExecutionContext());
        }

        public virtual ActionResult GetAuthorizationResult(ActionExecutionContext executionContext)
        {
            Initialize(executionContext.ControllerContext);
            return GetActionInvoker(executionContext).GetAuthorizationResult();
        }

        public virtual ModelStateDictionary ValidateRequest()
        {
            var executionContext = GetExecutionContext();
            Initialize(executionContext.ControllerContext);
            return GetActionInvoker(executionContext).ValidateRequest();
        }
    }

    [Serializable]
    public class ControllerActionException : Exception
    {
        public ControllerActionException(string message)
            : base(message)
        {
        }
    }
}