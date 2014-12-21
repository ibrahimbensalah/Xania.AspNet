using System;
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
            Expression<Func<TController, object>> actionExpression)
            where TController : ControllerBase
        {
            var actionInfo = new DirectControllerAction(controller, LinqActionDescriptor.Create(actionExpression));

            return actionInfo;
        }

        public static DirectControllerAction Action<TController>(this TController controller,
            Expression<Action<TController>> actionExpression, String httpMethod = "GET")
            where TController : ControllerBase
        {
            return new DirectControllerAction(controller, LinqActionDescriptor.Create(actionExpression))
            {
                HttpMethod = httpMethod
            };
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

        public static Router RegisterControllers(this Router application, params Assembly[] assemblies)
        {
            return RegisterControllers(application, null, assemblies);
        }

        public static Router RegisterControllers(this Router application,
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

        public static Router RegisterRoutes(this Router router, Action<RouteCollection> configAction)
        {
            configAction(router.Routes);
            return router;
        }

        public static IDictionary<string, object> ToDictionary(this object values)
        {
            return new RouteValueDictionary(values);
        } 
    }
}