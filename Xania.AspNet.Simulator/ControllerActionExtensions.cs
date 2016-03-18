using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xania.AspNet.Core;
using Xania.AspNet.Razor;

namespace Xania.AspNet.Simulator
{
    public static class ControllerActionExtensions
    {
        public static HttpContextBase CreateHttpContext(this IControllerAction actionRequest, ActionDescriptor actionDescriptor)
        {
            var controllerDescriptor = actionDescriptor.ControllerDescriptor;
            var controllerName = controllerDescriptor.ControllerName;

            var user = actionRequest.User ?? AspNetUtility.CreateAnonymousUser();
            var httpContext =
                AspNetUtility.GetContext(String.Format("/{0}/{1}", controllerName, actionDescriptor.ActionName),
                    actionRequest.HttpMethod, user);

            return httpContext;
        }

        public static DirectControllerAction PostAction<TController>(this TController controller,
            Expression<Func<TController, object>> actionExpression)
            where TController : ControllerBase
        {
            return Action(controller, actionExpression).Post();
        }

        public static DirectControllerAction GetAction<TController>(this TController controller,
            Expression<Func<TController, object>> actionExpression)
            where TController : ControllerBase
        {
            return Action(controller, actionExpression).Get();
        }

        public static DirectControllerAction Action<TController>(this TController controller,
            Expression<Func<TController, object>> actionExpression, string httpMethod = "GET")
            where TController : ControllerBase
        {

            return new DirectControllerAction(controller, LinqActionDescriptor.Create(actionExpression))
            {
                MvcApplication =
                {
                    ValueProvider = new LinqActionValueProvider(actionExpression.Body),
                },
                HttpMethod = httpMethod
            };
        }

        public static DirectControllerAction Action<TController>(this TController controller, string actionName, string httpMethod = "GET")
            where TController : ControllerBase
        {
            return new DirectControllerAction(controller, new LazyActionDescriptor(controller, actionName));
        }

        public static DirectControllerAction ChildAction<TController>(this TController controller,
            Expression<Func<TController, object>> actionExpression, string httpMethod = "GET")
            where TController : ControllerBase
        {
            var action = Action(controller, actionExpression, httpMethod);
            action.IsChildAction = true;

            return action;
        }

        public static RouteCollection GetDefaultRoutes()
        {
            var routes = new RouteCollection();
            if (RouteTable.Routes.Count == 0)
                routes.MapRoute(
                    "Default",
                    "{controller}/{action}/{id}",
                    new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                    );
            else
                foreach (var route in RouteTable.Routes)
                    routes.Add(route);

            return routes;
        }

        public static DirectControllerAction Action<TController>(this TController controller,
            Expression<Action<TController>> actionExpression, String httpMethod = "GET")
            where TController : ControllerBase
        {
            return new DirectControllerAction(controller, LinqActionDescriptor.Create(actionExpression))
            {
                MvcApplication =
                {
                    ValueProvider = new LinqActionValueProvider(actionExpression.Body)
                },
                HttpMethod = httpMethod
            };
        }

        public static TControllerAction AddCookie<TControllerAction>(this TControllerAction controllerAction, string name, string value)
            where TControllerAction: ControllerAction
        {
            controllerAction.Cookies.Add(new HttpCookie(name, value));
            return controllerAction;
        }

        public static TControllerAction AddSession<TControllerAction>(this TControllerAction controllerAction, string name, object value)
            where TControllerAction : ControllerAction
        {
            controllerAction.Session[name] = value;
            return controllerAction;
        }

        public static TService GetService<TService>(this IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService(typeof (TService));
            if (service is TService)
                return (TService) service;

            return Activator.CreateInstance<TService>();
        }

        public static ControllerContainer RegisterControllers(this ControllerContainer container, params Assembly[] assemblies)
        {
            return RegisterControllers(container, type => (ControllerBase)Activator.CreateInstance(type), assemblies);
        }

        public static ControllerContainer RegisterControllers(this ControllerContainer container,
            Func<Type, ControllerBase> factory, params Assembly[] assemblies)
        {
            const string controllerPostFix = "Controller";
            var controllerTypes =
                from t in ScanTypes(assemblies)
                where
                    typeof (ControllerBase).IsAssignableFrom(t) &&
                    t.Name.EndsWith(controllerPostFix, StringComparison.Ordinal)
                select t;

            foreach (var type in controllerTypes)
            {
                var controllerType = type;
                var name = controllerType.Name.Substring(0, controllerType.Name.Length - controllerPostFix.Length);

                container.RegisterController(name, ctx => factory(controllerType));
            }

            return container;
        }

        public static void ExecuteResult(this ActionResult actionResult, ActionExecutionContext executionContext)
        {
            new ControllerActionInvokerSimulator(executionContext.MvcApplication, executionContext)
                .InvokeActionResult(actionResult);
        }

        public static string RenderView(this DirectControllerAction controllerAction, Stream contentStream)
        {
            var stringWriter = new StringWriter();
            controllerAction.RenderView(contentStream, stringWriter);
            return stringWriter.ToString();
        }

        public static void RenderView(this DirectControllerAction controllerAction, Stream contentStream, TextWriter writer)
        {
            var actionName = controllerAction.ActionDescriptor.ActionName;
            var controllerName = controllerAction.ActionDescriptor.ControllerDescriptor.ControllerName;
            var virtualPath = string.Format("~/Views/{0}/{1}.cshtml", controllerName, actionName);

            var view = new RazorViewSimulator(controllerAction.MvcApplication, new StreamVirtualContent(virtualPath, contentStream));
            controllerAction.RenderView(view, writer);
        }

        public static string RenderView(this DirectControllerAction controllerAction, string viewName, string masterName)
        {
            var stringWriter = new StringWriter();
            controllerAction.RenderView(viewName, masterName, stringWriter);
            return stringWriter.ToString();
        }

        public static void RenderView(this DirectControllerAction controllerAction, string viewName, string masterName, TextWriter writer)
        {
            var controllerContext = controllerAction.CreateControllerContext();
            controllerContext.HttpContext.Response.Output = writer;

            var viewResult = new ViewResult
            {
                ViewName = viewName,
                MasterName = masterName,
                ViewEngineCollection = controllerAction.MvcApplication.ViewEngines
            };
            viewResult.ExecuteResult(controllerContext);
        }


        public static string RenderView(this DirectControllerAction controllerAction, IView view)
        {
            var stringWriter = new StringWriter();
            controllerAction.RenderView(view, stringWriter);
            return stringWriter.ToString();
        }

        public static void RenderView(this DirectControllerAction controllerAction, IView view, TextWriter writer)
        {
            var controllerContext = controllerAction.CreateControllerContext();
            controllerContext.HttpContext.Response.Output = writer;

            var viewResult = new ViewResult
            {
                View = view,
                ViewEngineCollection = controllerAction.MvcApplication.ViewEngines
            };
            viewResult.ExecuteResult(controllerContext);
        }

        public static ViewResultSimulator<object> View(this DirectControllerAction controllerAction, IVirtualContent virtualContent)
        {
            return new ViewResultSimulator<object>(controllerAction.GetExecutionContext(), virtualContent, null);
        }

        public static ViewResultSimulator<TModel> View<TModel>(this DirectControllerAction controllerAction, IVirtualContent virtualContent, TModel model)
        {
            return new ViewResultSimulator<TModel>(controllerAction.GetExecutionContext(), virtualContent, model);
        }

        public static ViewResultSimulator<TModel> View<TModel>(this DirectControllerAction controllerAction, ActionExecutionContext executionContext, IVirtualContent virtualContent, TModel model)
        {
            return new ViewResultSimulator<TModel>(executionContext, virtualContent, model);
        }

        public static AuthorizationContext GetAuthorizationContext(this ControllerAction controllerAction)
        {
            return controllerAction.GetExecutionContext().GetAuthorizationContext();
        }

        public static ActionExecutingContext GetActionExecutingContext(this ControllerAction controllerAction)
        {
            return controllerAction.GetExecutionContext().GetActionExecutingContext();
        }

        public static ActionExecutedContext GetActionExecutedContext(this ControllerAction controllerAction)
        {
            return controllerAction.GetExecutionContext().GetActionExecutedContext();
        }

        public static ResultExecutingContext GetResultExecutingContext(this ControllerAction controllerAction, ActionResult actionResult)
        {
            return controllerAction.GetExecutionContext().GetResultExecutingContext(actionResult);
        }

        public static ResultExecutedContext GetResultExecutedContext(this ControllerAction controllerAction, ActionResult actionResult, bool cancelled, Exception exception)
        {
            return controllerAction.GetExecutionContext().GetResultExecutedContext(actionResult, cancelled, exception);
        }

        public static ExceptionContext GetExceptionContext(this ControllerAction controllerAction, Exception exception)
        {
            return controllerAction.GetExecutionContext().GetExceptionContext(exception);
        }

        private static IEnumerable<Type> ScanTypes(params Assembly[] assemblies)
        {
            return assemblies.SelectMany(a => a.GetLoadedModules()).SelectMany(m => m.GetTypes())
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    !t.IsGenericTypeDefinition &&
                    !typeof (Delegate).IsAssignableFrom(t));
        }

        public static IDictionary<string, object> ToDictionary(this object values)
        {
            return new RouteValueDictionary(values);
        }
    }

    public class ViewResultSimulator<TModel>
    {
        private readonly ActionExecutionContext _executionContext;
        private readonly IVirtualContent _virtualContent;
        private readonly TModel _model;

        public ViewResultSimulator(ActionExecutionContext executionContext, IVirtualContent virtualContent, TModel model)
        {
            _executionContext = executionContext;
            _virtualContent = virtualContent;
            _model = model;
        }

        public TModel Model
        {
            get { return _model; }
        }

        public string ExecuteResult()
        {
            var writer = new StringWriter();
            ExecuteResult(writer);
            return writer.ToString();
        }

        public void ExecuteResult(TextWriter writer)
        {
            var view = new RazorViewSimulator(_executionContext.MvcApplication, _virtualContent);
            ExecuteResult(view, writer);
        }

        public virtual void ExecuteResult(IView view, TextWriter writer)
        {
            var viewData = _executionContext.Controller.ViewData;

            if (_model != null)
            {
                viewData.Model = _model;
            }

            var viewResult = new ViewResult
            {
                View = view,
                ViewData = _executionContext.ViewData,
                TempData = _executionContext.TempData,
                ViewEngineCollection = _executionContext.ViewEngines
            };

            new ControllerActionInvokerSimulator(_executionContext.MvcApplication, _executionContext)
                .InvokeActionResult(viewResult, writer);
        }
    }
}