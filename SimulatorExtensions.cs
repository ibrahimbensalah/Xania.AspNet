using System;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public static class SimulatorExtensions
    {
        public static ControllerAction Authenticate(this ControllerAction controllerAction, string userName, string[] roles, string identityType = "simulator")
        {
            var user = new GenericPrincipal(new GenericIdentity(userName, identityType), roles ?? new string[] { });
            controllerAction.Authenticate(user);
            return controllerAction;
        }

        public static ControllerAction Action<TController>(this TController controller, Expression<Func<TController, object>> actionExpression)
            where TController: ControllerBase
        {
            return new ControllerAction(controller, new LinqActionDescriptor<TController>(actionExpression));
        }

        public static ControllerActionResult Execute<TController>(this TController controller, Expression<Func<TController, object>> actionExpression)
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

        public static ControllerActionResult Execute<TController>(this IDependencyResolver resolver, Expression<Func<TController, object>> actionExpression)
            where TController : ControllerBase
        {
            return resolver.GetService<TController>().Execute(actionExpression);
        }
    }
}