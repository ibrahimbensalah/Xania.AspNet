using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xania.AspNet.Core;

namespace Xania.AspNet.Simulator
{
    public abstract class ControllerAction: IHttpRequest, IControllerAction
    {
        public IMvcApplication MvcApplication { get; private set; }
        public HttpContextBase HttpContext { get; set; }
        public bool IsChildAction { get; set; }

        protected ControllerAction(IMvcApplication mvcApplication, HttpContextBase context = null)
        {
            MvcApplication = mvcApplication;

            Cookies = new Collection<HttpCookie>();
            Session = new Dictionary<string, object>();

            HttpContext = context;
        }

        public IPrincipal User { get; set; }

        public ICollection<HttpCookie> Cookies { get; private set; }
        public IDictionary<string, object> Session { get; private set; }

        public string HttpMethod { get; set; }

        public string UriPath { get; set; }

        public abstract ActionExecutionContext GetExecutionContext();

        protected RouteData GetRouteData(HttpContextBase httpContext)
        {
            return MvcApplication.Routes.GetRouteData(httpContext);
        }

        protected virtual void Initialize(ControllerContext controllerContext)
        {
            var controller = controllerContext.Controller as Controller;
            controllerContext.Controller.ControllerContext = controllerContext;

            if (controller != null)
            {
                controller.ValueProvider = MvcApplication.GetValueProvider(controllerContext);
                controller.ControllerContext = controllerContext;
                controller.Url = new UrlHelper(controllerContext.RequestContext, MvcApplication.Routes);
                controller.ViewEngineCollection = MvcApplication.ViewEngines;
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

        internal ControllerActionInvokerSimulator GetActionInvoker(ActionExecutionContext actionExecutionContext)
        {
            return new ControllerActionInvokerSimulator(MvcApplication, actionExecutionContext);
        }

        public ActionResult Authorize()
        {
            return GetAuthorizationResult();
        }

        public virtual ActionResult GetAuthorizationResult()
        {
            var executionContext = GetExecutionContext();
            Initialize(executionContext.ControllerContext);
            return GetAuthorizationResult(executionContext);
        }

        public virtual ActionResult GetAuthorizationResult(ActionExecutionContext executionContext)
        {
            return GetActionInvoker(executionContext).GetAuthorizationResult();
        }

        public virtual ModelStateDictionary ValidateRequest()
        {
            var executionContext = GetExecutionContext();
            Initialize(executionContext.ControllerContext);
            return ValidateRequest(executionContext);
        }

        public virtual ModelStateDictionary ValidateRequest(ActionExecutionContext executionContext)
        {
            return GetActionInvoker(executionContext).ValidateRequest();
        }
    }
}