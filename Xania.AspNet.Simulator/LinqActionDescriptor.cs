using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    internal class LinqActionDescriptor<TController> : ActionDescriptor
        where TController: ControllerBase
    {
        private readonly Func<TController, object> _actionDelegate;
        private readonly string _actionName;
        private readonly IEnumerable<FilterAttribute> _filters;
        private readonly ICollection<ActionSelector> _selectors;

        public LinqActionDescriptor(Func<TController, object> actionDelegate, MethodInfo method)
        {
            _actionDelegate = actionDelegate;
            _actionName = method.Name;

            var attributes = method.GetCustomAttributes().ToArray();
         
            _filters = attributes.OfType<FilterAttribute>();
            _selectors =
                attributes.OfType<ActionMethodSelectorAttribute>()
                    .Select(ams => new ActionSelector((context) => ams.IsValidForRequest(context, method))).ToList();
        }

        public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters)
        {
            var controller = controllerContext.Controller as TController;

            if (controller == null)
                throw new InvalidOperationException("Cannot invoke action on controller, expected controller of type " +
                    typeof(TController) + ", but was " + controllerContext.Controller.GetType());

            controller.ControllerContext = controllerContext;
            return _actionDelegate(controller);
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
            return _filters ?? Enumerable.Empty<FilterAttribute>();
        }

        public override ICollection<ActionSelector> GetSelectors()
        {
            return _selectors ?? new ActionSelector[]{};
        }
    }
}
