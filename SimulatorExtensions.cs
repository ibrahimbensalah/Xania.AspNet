using System;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public static class SimulatorExtensions
    {
        public static void Authenticate(this IMvcRequest request, string userName, string identityType = "simulator")
        {
            var user = new GenericPrincipal(new GenericIdentity(userName, identityType), new string[] { });
            request.Authenticate(user);
        }

        public static MvcResult Execute<TController>(this TController controller, Expression<Func<TController, object>> actionExpression)
            where TController: ControllerBase
        {
            return new MvcRequest<TController>(controller, actionExpression).Execute();
        }

        public static TService GetService<TService>(this IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService(typeof (TService));
            if (service is TService)
                return (TService) service;

            return Activator.CreateInstance<TService>();
        }

        public static MvcResult Execute<TController>(this IDependencyResolver resolver, Expression<Func<TController, object>> actionExpression)
            where TController : ControllerBase
        {
            return resolver.GetService<TController>().Execute(actionExpression);
        }
    }
}