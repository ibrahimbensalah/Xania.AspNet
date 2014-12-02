﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public static class SimulatorExtensions
    {
        public static ControllerAction Authenticate(this ControllerAction controllerAction, string userName,
            string[] roles, string identityType = "simulator")
        {
            var user = new GenericPrincipal(new GenericIdentity(userName, identityType), roles ?? new string[] {});
            controllerAction.Authenticate(user);
            return controllerAction;
        }

        public static ControllerAction Action<TController>(this TController controller,
            Expression<Func<TController, object>> actionExpression)
            where TController : ControllerBase
        {
            var methodCallExpression = (MethodCallExpression)actionExpression.Body;
            var actionDelegate = actionExpression.Compile();
            return new ControllerAction(controller, new LinqActionDescriptor<TController>(actionDelegate, methodCallExpression.Method));
        }

        public static ControllerAction Action<TController>(this TController controller,
            Expression<Action<TController>> actionExpression, String httpMethod = "GET")
            where TController : ControllerBase
        {
            var methodCallExpression = (MethodCallExpression)actionExpression.Body;
            Func<TController, object> actionDelegate = c =>
            {
                actionExpression.Compile()(c);
                return null;
            };

            return new ControllerAction(controller, new LinqActionDescriptor<TController>(actionDelegate, methodCallExpression.Method), httpMethod);
        }

        public static ControllerActionResult Execute<TController>(this TController controller,
            Expression<Func<TController, object>> actionExpression)
            where TController : ControllerBase
        {
            return Action(controller, actionExpression).Execute();
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

        public static MvcApplication RegisterControllers(this MvcApplication application, params Assembly[] assemblies)
        {
            return RegisterControllers(application, null, assemblies);
        }

        public static MvcApplication RegisterControllers(this MvcApplication application,
            IDependencyResolver dependencyResolver, params Assembly[] assemblies)
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
                var name = type.Name.Substring(0, type.Name.Length - controllerPostFix.Length);
                var instance = (dependencyResolver == null)
                    ? Activator.CreateInstance(type)
                    : dependencyResolver.GetService(type);

                application.RegisterController(name, (ControllerBase) instance);
            }

            return application;
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
    }
}