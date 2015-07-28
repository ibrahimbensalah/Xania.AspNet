using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    internal class SimulatorActionInvoker : ControllerActionInvoker
    {
        private readonly ControllerContext _controllerContext;
        private readonly ActionDescriptor _actionDescriptor;
        private readonly RouteCollection _routes;
        private readonly FilterInfo _filterInfo;

        public SimulatorActionInvoker(ActionExecutionContext actionExecutionContext, IEnumerable<Filter> filters, RouteCollection routes)
        {
            var enumerable = filters as Filter[] ?? filters.ToArray();
            SimulatorHelper.InitializeFilters(enumerable);

            _controllerContext = actionExecutionContext.ControllerContext;
            _actionDescriptor = actionExecutionContext.ActionDescriptor;
            _routes = routes;
            _filterInfo = new FilterInfo(enumerable);
        }

        public virtual ActionResult GetAuthorizationResult()
        {
            var authorizationContext = InvokeAuthorizationFilters(_controllerContext,
                _filterInfo.AuthorizationFilters, _actionDescriptor);

            return authorizationContext.Result;
        }

        protected override AuthorizationContext InvokeAuthorizationFilters(ControllerContext controllerContext, IList<IAuthorizationFilter> filters,
            ActionDescriptor actionDescriptor)
        {
            var authorizationContext = base.InvokeAuthorizationFilters(controllerContext, filters, actionDescriptor);
            SimulatorHelper.InitizializeActionResults(authorizationContext.Result, _routes);
            return authorizationContext;
        }

        public virtual ActionResult GetActionResult()
        {
            var parameters = GetParameterValues(_controllerContext, _actionDescriptor);
            var actionExecutedContext = InvokeActionMethodWithFilters(_controllerContext,
                _filterInfo.ActionFilters, _actionDescriptor, parameters);

            if (actionExecutedContext == null)
                throw new Exception("GetActionResult returned null");

            SimulatorHelper.InitizializeActionResults(actionExecutedContext.Result, _routes);
            return actionExecutedContext.Result;
        }

        protected override object GetParameterValue(ControllerContext controllerContext, ParameterDescriptor parameterDescriptor)
        {
            var parameterName = parameterDescriptor.ParameterName;
            var value = base.GetParameterValue(controllerContext, parameterDescriptor);

            if (value == null)
                return null;

            var validationResults = ValidateModel(parameterDescriptor.ParameterType, value, controllerContext);

            var modelState = controllerContext.Controller.ViewData.ModelState;
            Func<ModelValidationResult, bool> isValidField = res => modelState.IsValidField(String.Format("{0}.{1}", parameterName, res.MemberName));

            foreach (var validationResult in validationResults.Where(isValidField).ToArray())
            {
                var subPropertyName = String.Format("{0}.{1}", parameterName, validationResult.MemberName);
                modelState.AddModelError(subPropertyName, validationResult.Message);
            }

            return value;
        }

        protected virtual IEnumerable<ModelValidationResult> ValidateModel(Type modelType, object modelValue, ControllerContext controllerContext)
        {
            var modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => modelValue, modelType);
            return ModelValidator.GetModelValidator(modelMetadata, controllerContext).Validate(null);
        }

        public virtual ModelStateDictionary ValidateRequest()
        {
            var parameters = GetParameterValues(_controllerContext, _actionDescriptor);
            return _controllerContext.Controller.ViewData.ModelState;
        }
    }
}