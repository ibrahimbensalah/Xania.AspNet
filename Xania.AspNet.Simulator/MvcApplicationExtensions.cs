using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using Xania.AspNet.Core;

namespace Xania.AspNet.Simulator
{
    public static class MvcApplicationExtensions
    {
        public static DirectControllerAction Action(this IMvcApplication mvcApplication, ControllerBase controller, string actionName)
        {
            return new DirectControllerAction(mvcApplication, controller, new LazyActionDescriptor(controller, actionName));
        }

        public static DirectControllerAction Action(this IMvcApplication mvcApplication, string controllerName, string actionName)
        {
            var controller = mvcApplication.ControllerFactory.CreateController(null, controllerName);
            return new DirectControllerAction(mvcApplication, controller, new LazyActionDescriptor(controller, actionName));
        }

    }
}
