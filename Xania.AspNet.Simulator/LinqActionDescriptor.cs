using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    internal class LinqActionDescriptor : ReflectedActionDescriptor
    {
        private readonly Func<ControllerBase, object> _executeFunc;
        private readonly MethodCallExpression _methodCallExpression;

        private LinqActionDescriptor(Func<ControllerBase, object> executeFunc, MethodCallExpression methodCallExpression, ReflectedControllerDescriptor controllerDescriptor)
            : base(methodCallExpression.Method, methodCallExpression.Method.Name, controllerDescriptor)
        {
            _executeFunc = executeFunc;
            _methodCallExpression = methodCallExpression;
        }

        public static LinqActionDescriptor Create<TController>(Expression<Func<TController, object>> actionExpression)
            where TController: ControllerBase
        {
            Func<ControllerBase, object> executeFunc = controller => actionExpression.Compile().Invoke((TController)controller);

            var methodCallExpression = (MethodCallExpression)actionExpression.Body;

            return new LinqActionDescriptor(executeFunc, methodCallExpression, new ReflectedControllerDescriptor(typeof(TController)));
        }

        public static LinqActionDescriptor Create<TController>(Expression<Action<TController>> actionExpression)
            where TController: ControllerBase
        {
            Func<ControllerBase, object> executeFunc = c =>
            {
                actionExpression.Compile().Invoke((TController)c);
                return null;
            };

            var methodCallExpression = (MethodCallExpression)actionExpression.Body;

            return new LinqActionDescriptor(executeFunc, methodCallExpression, new ReflectedControllerDescriptor(typeof(TController)));
        }

        public override ParameterDescriptor[] GetParameters()
        {
            return new ParameterDescriptor[0];
        }

        public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters)
        {
            foreach(var parameter in base.GetParameters().Select( (p, i) => new { Name = p.ParameterName, Index = i, Type = p.ParameterType }))
            {
                var value = Invoke(_methodCallExpression.Arguments[parameter.Index]);
                if (value != null)
                    ValidateArgument(parameter.Name, parameter.Type, value, controllerContext);
            }

            return _executeFunc(controllerContext.Controller);
        }

        protected virtual void ValidateArgument(string parameterName, Type parameterType, object parameterValue, ControllerContext controllerContext)
        {
            var validationResults = ValidateModel(parameterType, parameterValue, controllerContext);
            
            var modelState = controllerContext.Controller.ViewData.ModelState;
            Func<ModelValidationResult, bool> isValidField = res => modelState.IsValidField(String.Format("{0}.{1}", parameterName, res.MemberName));

            foreach (var validationResult in validationResults.Where(isValidField).ToArray())
            {
                var subPropertyName = String.Format("{0}.{1}", parameterName, validationResult.MemberName);
                modelState.AddModelError(subPropertyName, validationResult.Message);
            }

        }

        protected virtual IEnumerable<ModelValidationResult> ValidateModel(Type modelType, object modelValue, ControllerContext controllerContext)
        {
            var modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => modelValue, modelType);
            return ModelValidator.GetModelValidator(modelMetadata, controllerContext).Validate(null);
        }

        private static object Invoke(Expression valueExpression)
        {
            var convertExpression = Expression.Convert(valueExpression, typeof (object));
            var express = Expression.Lambda<Func<object>>(convertExpression).Compile();
            return express.Invoke();
        }
    }
}