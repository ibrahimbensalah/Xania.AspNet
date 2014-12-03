using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    internal class LinqParameterDescriptor : ParameterDescriptor
    {
        private readonly ParameterDescriptor _inner;
        private readonly Expression _valueExpression;

        public LinqParameterDescriptor(ParameterDescriptor inner, Expression valueExpression)
        {
            _inner = inner;
            _valueExpression = valueExpression;
        }

        public override ParameterBindingInfo BindingInfo
        {
            get { return new LinqParameterBindingInfo(_valueExpression); }
        }

        public override object DefaultValue
        {
            get { return _inner.DefaultValue; }
        }

        public override bool Equals(object obj)
        {
            return _inner.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _inner.GetHashCode();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _inner.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _inner.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _inner.IsDefined(attributeType, inherit);
        }

        public override string ToString()
        {
            return _inner.ToString();
        }

        public override ActionDescriptor ActionDescriptor
        {
            get { return _inner.ActionDescriptor; }
        }

        public override string ParameterName
        {
            get { return _inner.ParameterName; }
        }

        public override Type ParameterType
        {
            get { return _inner.ParameterType; }
        }
    }

    internal class LinqParameterBindingInfo : ParameterBindingInfo, IModelBinder
    {
        private readonly Expression _valueExpression;

        public LinqParameterBindingInfo(Expression valueExpression)
        {
            _valueExpression = valueExpression;
        }

        public override IModelBinder Binder
        {
            get { return this; }
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            bindingContext.ModelMetadata.Model = GetValue();

            Validate(controllerContext, bindingContext);

            return bindingContext.ModelMetadata.Model;
        }

        private static void Validate(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var startedValid = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            foreach (
                ModelValidationResult validationResult in
                    ModelValidator.GetModelValidator(bindingContext.ModelMetadata, controllerContext).Validate(null))
            {
                string subPropertyName = CreateSubPropertyName(bindingContext.ModelName, validationResult.MemberName);

                if (!startedValid.ContainsKey(subPropertyName))
                {
                    startedValid[subPropertyName] = bindingContext.ModelState.IsValidField(subPropertyName);
                }

                if (startedValid[subPropertyName])
                {
                    bindingContext.ModelState.AddModelError(subPropertyName, validationResult.Message);
                }
            }
        }

        private object GetValue()
        {
            var express = Expression.Lambda<Func<object>>(_valueExpression).Compile();
            var value = express.Invoke();
            return value;
        }


        protected internal static string CreateSubPropertyName(string prefix, string propertyName)
        {
            if (String.IsNullOrEmpty(prefix))
            {
                return propertyName;
            }
            else if (String.IsNullOrEmpty(propertyName))
            {
                return prefix;
            }
            else
            {
                return prefix + "." + propertyName;
            }
        }
    }
}