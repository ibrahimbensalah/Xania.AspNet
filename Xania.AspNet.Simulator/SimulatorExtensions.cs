using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public static class ControlllerActionExtensions
    {
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
            where TController: ControllerBase
        {
            var controllerDesc = new ReflectedControllerDescriptor(typeof (TController));
            return new DirectControllerAction(controller, new LazyActionDescriptor<TController>(controller, actionName));
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

        public static TControllerAction AddSession<TControllerAction>(this TControllerAction controllerAction, string name, string value)
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
            actionResult.ExecuteResult(executionContext.ControllerContext);
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

    public class LazyActionDescriptor<TController> : ActionDescriptor
        where TController: ControllerBase
    {
        private readonly TController _controller;
        private readonly string _actionName;
        private ActionDescriptor _inner;
        private ReflectedControllerDescriptor _controllerDesc;

        public LazyActionDescriptor(TController controller, string actionName)
        {
            _controller = controller;
            _actionName = actionName;
        }

        public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters)
        {
            return Inner.Execute(controllerContext, parameters);
        }

        public override ParameterDescriptor[] GetParameters()
        {
            return Inner.GetParameters();
        }

        public override string ActionName
        {
            get { return _actionName; }
        }

        public override ControllerDescriptor ControllerDescriptor
        {
            get
            {
                if (_controllerDesc == null)
                {
                    _controllerDesc = new ReflectedControllerDescriptor(typeof (TController));
                }
                return _controllerDesc;
            }
        }

        private ActionDescriptor Inner
        {
            get
            {
                if (_inner == null)
                {
                    _inner = ControllerDescriptor.FindAction(_controller.ControllerContext, _actionName);
                    if (_inner == null)
                        throw new HttpException(404, "Action '" + _actionName + "' of controller '" + ControllerDescriptor.ControllerName + "' not found");
                }
                return _inner;
            }
        }
    }

    public class LinqActionValueProvider : IValueProvider
    {
        private readonly Dictionary<string, object> _values;

        public LinqActionValueProvider(Expression body)
        {
            _values = new Dictionary<String, object>();

            var methodCallExpression = (MethodCallExpression)body;
            var methodParameters = methodCallExpression.Method.GetParameters();
            for (int i=0 ; i < methodCallExpression.Arguments.Count ; i++)
            {
                var arg = methodCallExpression.Arguments[i];
                var par = methodParameters[i];
                _values.Add(par.Name, Invoke(arg));
            }
        }
        private static object Invoke(Expression valueExpression)
        {
            var convertExpression = Expression.Convert(valueExpression, typeof(object));
            var express = Expression.Lambda<Func<object>>(convertExpression).Compile();
            return express.Invoke();
        }
        public bool ContainsPrefix(string prefix)
        {
            return _values.ContainsKey(prefix);
        }

        public ValueProviderResult GetValue(string key)
        {
            return new ValueProviderResult(_values[key], null, CultureInfo.InvariantCulture);
        }
    }
}