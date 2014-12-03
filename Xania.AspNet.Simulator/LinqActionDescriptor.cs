using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    internal class LinqActionDescriptor : ReflectedActionDescriptor
    {
        private readonly MethodCallExpression _methodCallExpression;

        public LinqActionDescriptor(MethodCallExpression methodCallExpression, ReflectedControllerDescriptor controllerDescriptor)
            : base(methodCallExpression.Method, methodCallExpression.Method.Name, controllerDescriptor)
        {
            _methodCallExpression = methodCallExpression;
        }

        public override ParameterDescriptor[] GetParameters()
        {
            return Wrap().ToArray();
        }

        private IEnumerable<ParameterDescriptor> Wrap() 
        {
            var parameters = base.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                yield return new LinqParameterDescriptor(parameters[i], _methodCallExpression.Arguments[i]);
            }
        }
    }
}