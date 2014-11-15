using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Xania.AspNet
{
    public class FactoryActionDescriptor : ActionDescriptor
    {
        private readonly Type _controllerType;
        private readonly string _actionName;
        private readonly Func<ControllerBase, object> _action;

        private FactoryActionDescriptor(Type controllerType, String actionName, Func<ControllerBase, object> action)
        {
            _controllerType = controllerType;
            _actionName = actionName;
            _action = action;
        }

        public static FactoryActionDescriptor Create<TController>(Expression<Func<TController, object>> actionExpression)
            where TController : ControllerBase
        {
            var methodCallExpression = actionExpression.Body as MethodCallExpression;
            if (methodCallExpression == null)
                throw new ArgumentException("actionExpression is not a method call expression like: ctrl => ctrl.DoAction(...)");

            Func<ControllerBase, object> actionFunc = controller =>
            {
                if (controller == null) throw new ArgumentNullException("controller");

                if (controller is TController)
                    return actionExpression.Compile().Invoke(controller as TController);

                throw new InvalidOperationException("Cannot invoke action on controller, expected controller of type " +
                                                    typeof(TController) + ", but was " + controller.GetType());
            };

            var actionName = methodCallExpression.Method.Name;
            return new FactoryActionDescriptor(typeof(TController), actionName, actionFunc);
        }

        public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters)
        {
            return _action(controllerContext.Controller);
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
            get { return new ReflectedControllerDescriptor(_controllerType); }
        }
    }
}
