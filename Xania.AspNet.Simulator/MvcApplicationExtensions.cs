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
            return new DirectControllerAction(mvcApplication, controller, new LazyActionDescriptor(controller, actionName));
        }
    }
}
