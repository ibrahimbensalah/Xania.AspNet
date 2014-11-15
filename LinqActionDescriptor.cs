using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Xania.AspNet
{
    public class LinqActionDescriptor<TController> : ActionDescriptor
        where TController: ControllerBase
    {
        private readonly Expression<Func<TController, object>> _actionExpression;

        private readonly string _actionName;
        private readonly IEnumerable<FilterAttribute> _filterAttributes;

        public LinqActionDescriptor(Expression<Func<TController, object>> actionExpression)
        {
            _actionExpression = actionExpression;

            var methodCallExpression = actionExpression.Body as MethodCallExpression;
            if (methodCallExpression == null)
                throw new ArgumentException("actionExpression is not a method call expression like: ctrl => ctrl.DoAction(...)");

            var actionName = methodCallExpression.Method.Name;
            _actionName = actionName;

            var filterAttributes = methodCallExpression.Method.GetCustomAttributes(true).OfType<FilterAttribute>();
            _filterAttributes = filterAttributes;
        }

        public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters)
        {
            var controller = controllerContext.Controller;

            if (controller is TController)
                return _actionExpression.Compile().Invoke((TController)controllerContext.Controller);

            throw new InvalidOperationException("Cannot invoke action on controller, expected controller of type " +
                                                typeof(TController) + ", but was " + controller.GetType());
        }

        public override ParameterDescriptor[] GetParameters()
        {
            return new ParameterDescriptor[0];
        }

        public override string ActionName
        {
            get { return _actionName; }
        }

        public override ControllerDescriptor ControllerDescriptor
        {
            get { return new ReflectedControllerDescriptor(typeof(TController)); }
        }

        public override IEnumerable<FilterAttribute> GetFilterAttributes(bool useCache)
        {
            return _filterAttributes;
        }
    }
}
