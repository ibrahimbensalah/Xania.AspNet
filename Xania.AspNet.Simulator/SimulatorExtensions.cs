using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class LazyActionDescriptor : ActionDescriptor
    {
        private readonly ControllerBase _controller;
        private readonly string _actionName;
        private ActionDescriptor _inner;
        private ReflectedControllerDescriptor _controllerDesc;

        public LazyActionDescriptor(ControllerBase controller, string actionName)
        {
            _controller = controller;
            _actionName = actionName;
        }

        public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters)
        {
            return Inner.Execute(controllerContext, parameters);
        }

        public override ParameterDescriptor[] GetParameters()
        {
            return Inner.GetParameters();
        }

        public override string ActionName
        {
            get { return _actionName; }
        }

        public override ControllerDescriptor ControllerDescriptor
        {
            get
            {
                if (_controllerDesc == null)
                {
                    _controllerDesc = new ReflectedControllerDescriptor(_controller.GetType());
                }
                return _controllerDesc;
            }
        }

        private ActionDescriptor Inner
        {
            get
            {
                if (_inner == null)
                {
                    _inner = ControllerDescriptor.FindAction(_controller.ControllerContext, _actionName);
                    if (_inner == null)
                        throw new HttpException(404, "Action '" + _actionName + "' of controller '" + ControllerDescriptor.ControllerName + "' not found");
                }
                return _inner;
            }
        }

        public override IEnumerable<FilterAttribute> GetFilterAttributes(bool useCache)
        {
            return Inner.GetFilterAttributes(useCache);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return Inner.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return Inner.GetCustomAttributes(inherit);
        }

        public override ICollection<ActionSelector> GetSelectors()
        {
            return Inner.GetSelectors();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return Inner.IsDefined(attributeType, inherit);
        }

        public override string UniqueId
        {
            get { return Inner.UniqueId; }
        }

        public override int GetHashCode()
        {
            return Inner.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is LazyActionDescriptor && Equals((LazyActionDescriptor) obj);
        }

        public virtual bool Equals(LazyActionDescriptor lazyActionDescriptor)
        {
            return Inner.Equals(lazyActionDescriptor.Inner);
        }

        public override string ToString()
        {
            return Inner.ToString();
        }
    }
    public class LinqActionValueProvider : IValueProvider
    {
        private readonly Dictionary<string, object> _values;

        public LinqActionValueProvider(Expression body)
        {
            _values = new Dictionary<String, object>();

            var methodCallExpression = (MethodCallExpression)body;
            var methodParameters = methodCallExpression.Method.GetParameters();
            for (int i=0 ; i < methodCallExpression.Arguments.Count ; i++)
            {
                var arg = methodCallExpression.Arguments[i];
                var par = methodParameters[i];
                _values.Add(par.Name, Invoke(arg));
            }
        }
        private static object Invoke(Expression valueExpression)
        {
            var convertExpression = Expression.Convert(valueExpression, typeof(object));
            var express = Expression.Lambda<Func<object>>(convertExpression).Compile();
            return express.Invoke();
        }
        public bool ContainsPrefix(string prefix)
        {
            return _values.ContainsKey(prefix);
        }

        public ValueProviderResult GetValue(string key)
        {
            return new ValueProviderResult(_values[key], null, CultureInfo.InvariantCulture);
        }
    }
}