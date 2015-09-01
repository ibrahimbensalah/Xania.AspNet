using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xania.AspNet.Core;
using Xania.AspNet.Simulator.Http;

namespace Xania.AspNet.Simulator
{
    public class DirectControllerAction : ControllerAction
    {
        public DirectControllerAction(ControllerBase controller, ActionDescriptor actionDescriptor)
            : this(Simulator.MvcApplication.CreateDefault(), controller, actionDescriptor)
        {
        }

        public DirectControllerAction(IMvcApplication mvcApplication, ControllerBase controller, ActionDescriptor actionDescriptor)
            : base(mvcApplication)
        {
            Controller = controller;
            ActionDescriptor = actionDescriptor;
        }

        public virtual ControllerBase Controller { get; private set; }

        public virtual ActionDescriptor ActionDescriptor { get; private set; }

        public override ActionExecutionContext GetExecutionContext()
        {
            var controllerContext = CreateControllerContext();

            return new ActionExecutionContext
            {
                ControllerContext = controllerContext,
                ActionDescriptor = ActionDescriptor
            };
        }

        public virtual HttpContextBase CreateHttpContext()
        {
            return HttpContext ?? this.CreateHttpContext(ActionDescriptor);
        }

        public virtual ControllerContext CreateControllerContext()
        {
            var httpContext = new HttpContextDecorator(CreateHttpContext());
            return CreateControllerContext(httpContext, Controller, ActionDescriptor);
        }

        protected virtual ControllerContext CreateControllerContext(HttpContextBase httpContext, ControllerBase controller, ActionDescriptor actionDescriptor)
        {
            var requestContext = GetRequestContext(httpContext, actionDescriptor);
            var controllerContext = new ControllerContext(requestContext, controller);
            controller.ControllerContext = controllerContext;

            if (actionDescriptor.GetSelectors().Any(selector => !selector.Invoke(controllerContext)))
            {
                throw new HttpException(404, String.Format("Http method '{0}' is not allowed", controllerContext.HttpContext.Request.HttpMethod));
            }

            Initialize(controllerContext);

            return controllerContext;
        }

        private RequestContext GetRequestContext(HttpContextBase httpContext, ActionDescriptor actionDescriptor)
        {
            var controllerName = actionDescriptor.ControllerDescriptor.ControllerName;
            var actionName = actionDescriptor.ActionName;
            var routeData = new RouteData { Values = { { "controller", controllerName }, { "action", actionName } } };

            var requestContext = new RequestContext(httpContext, routeData);

            foreach (var cookie in Cookies)
                requestContext.HttpContext.Request.Cookies.Add(cookie);

            foreach (var kvp in Session)
                // ReSharper disable once PossibleNullReferenceException
                requestContext.HttpContext.Session[kvp.Key] = kvp.Value;

            if (IsChildAction)
            {
                const string parentActionViewContextToken = "ParentActionViewContext";
                requestContext.RouteData.DataTokens.Add(parentActionViewContextToken, new ViewContext());
            }
            return requestContext;
        }
    }

    public class EmptyView : IView
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}