﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        //public static IAction Authenticate(this IAction action, string userName,
        //    string[] roles, string identityType = "simulator")
        //{
        //    var user = new GenericPrincipal(new GenericIdentity(userName, identityType), roles ?? new string[] {});
        //    action.Authenticate(user);
        //    return action;
        //}

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
                ValueProvider = new LinqActionValueProvider(actionExpression.Body),
                HttpMethod = httpMethod
            };
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
                ValueProvider = new LinqActionValueProvider(actionExpression.Body),
                HttpMethod = httpMethod
            };
        }

        public static ControllerActionResult Execute<TController>(this TController controller,
            Expression<Func<TController, object>> actionExpression)
            where TController : ControllerBase
        {
            var action = Action(controller, actionExpression);
            return action.Invoke();
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

        public static TControllerAction AddFile<TControllerAction>(this TControllerAction controllerAction, string name,
            Stream stream) where TControllerAction: ControllerAction
        {
            controllerAction.Files[name] = stream;
            return controllerAction;
        }

        public static TService GetService<TService>(this IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService(typeof (TService));
            if (service is TService)
                return (TService) service;

            return Activator.CreateInstance<TService>();
        }

        public static ControllerActionResult Execute<TController>(this IDependencyResolver resolver,
            Expression<Func<TController, object>> actionExpression)
            where TController : ControllerBase
        {
            return resolver.GetService<TController>().Execute(actionExpression);
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