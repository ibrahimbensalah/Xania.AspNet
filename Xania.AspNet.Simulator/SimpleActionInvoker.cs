using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    internal class SimpleActionInvoker : ControllerActionInvoker
    {
        private readonly ControllerContext _controllerContext;
        private readonly ActionDescriptor _actionDescriptor;
        private readonly FilterInfo _filterInfo;

        public SimpleActionInvoker(ControllerContext controllerContext, ActionDescriptor actionDescriptor, IEnumerable<Filter> filters)
        {
            _controllerContext = controllerContext;
            _actionDescriptor = actionDescriptor;
            _filterInfo = new FilterInfo(filters);
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
            var parameters = GetParameterValues(_controllerContext, _actionDescriptor);
            var actionExecutedContext = InvokeActionMethodWithFilters(_controllerContext,
                _filterInfo.ActionFilters, _actionDescriptor, parameters);

            if (actionExecutedContext == null)
                throw new Exception("InvokeActionMethodWithFilters returned null");

            return actionExecutedContext.Result;
        }
        
    }
}