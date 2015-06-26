using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using Xania.AspNet.Core;

namespace Xania.AspNet.Simulator
{
    internal static class MvcApplicationExtensions
    {
        public static DirectControllerAction Action(this ControllerBase controller, IMvcApplication mvcApplication, string actionName)
        {
            var controllerType = controller.GetType();
            var controllerDescriptor = new ReflectedControllerDescriptor(controllerType);
            var actionDescriptor = controllerDescriptor.FindAction(controller.ControllerContext, actionName);

            return new DirectControllerAction(mvcApplication, controller, actionDescriptor)
            {
                Output = new StringWriter()
            };
        }
    }

    internal class NotNullAttribute : Attribute
    {
    }
}
