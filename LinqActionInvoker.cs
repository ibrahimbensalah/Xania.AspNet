using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class LinqActionInvoker : ControllerActionInvoker
    {
        public virtual ActionResult InvokeAction<TController>(ControllerContext controllerContext, Expression<Func<TController, object>> actionExpression)
            where TController : ControllerBase 
        {
            var actionDescriptor = CreateActionDescriptor(actionExpression);

            var globalFilters = new GlobalFilterCollection();
            // FilterConfig.RegisterGlobalFilters(globalFilters);
            FilterAttributeFilterProvider c = null;

            var filters =
                FilterProviders.Providers.GetFilters(controllerContext, actionDescriptor)
                    .Concat(globalFilters);
            var filterInfo = new FilterInfo(filters); // GetFilters(controller.ControllerContext, actionDescriptor);

            var authorizationContext = InvokeAuthorizationFilters(controllerContext,
                filterInfo.AuthorizationFilters, actionDescriptor);

            if (authorizationContext.Result != null)
            {
                InvokeAuthenticationFiltersChallenge(
                    controllerContext, filterInfo.AuthenticationFilters, actionDescriptor,
                    authorizationContext.Result);
                return authorizationContext.Result;
            }

            var dummyParameters = new Dictionary<string, object>();
            var actionExecutedContext = InvokeActionMethodWithFilters(controllerContext,
                filterInfo.ActionFilters,
                actionDescriptor, dummyParameters);
            if (actionExecutedContext == null)
                throw new Exception("InvokeActionMethodWithFilters returned null");

            return actionExecutedContext.Result;
        }

        private static LinqActionDescriptor<TController> CreateActionDescriptor<TController>(Expression<Func<TController, object>> actionExpression)
            where TController : ControllerBase
        {
            return new LinqActionDescriptor<TController>(actionExpression);
        }
    }
}