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

        private void ValidateArgument(string parameterName, Type parameterType, object parameterValue, ControllerContext controllerContext)
        {
            var modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => parameterValue, parameterType);
            var modelState = controllerContext.Controller.ViewData.ModelState;

            var startedValid = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            foreach (
                var validationResult in
                    ModelValidator.GetModelValidator(modelMetadata, controllerContext).Validate(null))
            {
                string subPropertyName = String.Format("{0}.{1}", parameterName, validationResult.MemberName);

                if (!startedValid.ContainsKey(subPropertyName))
                {
                    startedValid[subPropertyName] = modelState.IsValidField(subPropertyName);
                }

                if (startedValid[subPropertyName])
                {
                    modelState.AddModelError(subPropertyName, validationResult.Message);
                }
            }

        }

        private static object Invoke(Expression valueExpression)
        {
            var express = Expression.Lambda<Func<object>>(valueExpression).Compile();
            return express.Invoke();
        }
    }
}