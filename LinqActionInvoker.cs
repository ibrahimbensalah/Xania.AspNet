using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class LinqActionInvoker<TController> : ControllerActionInvoker
        where TController : ControllerBase
    {
        private readonly ControllerContext _controllerContext;
        private readonly LinqActionDescriptor<TController> _actionDescriptor;
        private readonly FilterInfo _filterInfo;

        public LinqActionInvoker(ControllerContext controllerContext,
            Expression<Func<TController, object>> actionExpression)
        {
            _controllerContext = controllerContext;
            _actionDescriptor = new LinqActionDescriptor<TController>(actionExpression);
            _filterInfo = GetFilterInfo();
        }

        private ActionResult AuthorizeAction()
        {
            var authorizationContext = InvokeAuthorizationFilters(_controllerContext,
                _filterInfo.AuthorizationFilters, _actionDescriptor);

            var authorizationResult = authorizationContext.Result;
            if (authorizationResult != null)
            {
                InvokeAuthenticationFiltersChallenge(_controllerContext, _filterInfo.AuthenticationFilters, _actionDescriptor, authorizationResult);
            }

            return authorizationResult;
        }

        public virtual ActionResult InvokeAction()
        {
            return AuthorizeAction() ?? InvokeActionMethodWithFilters();
        }

        private ActionResult InvokeActionMethodWithFilters()
        {
            var dummyParameters = new Dictionary<string, object>();
            var actionExecutedContext = InvokeActionMethodWithFilters(_controllerContext,
                _filterInfo.ActionFilters, _actionDescriptor, dummyParameters);

            if (actionExecutedContext == null)
                throw new Exception("InvokeActionMethodWithFilters returned null");

            return actionExecutedContext.Result;
        }

        private FilterInfo GetFilterInfo()
        {
            var filters = FilterProviders.Providers.GetFilters(_controllerContext, _actionDescriptor);
            return new FilterInfo(filters);
        }
    }
}