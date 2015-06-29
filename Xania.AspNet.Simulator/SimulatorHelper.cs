using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    static internal class SimulatorHelper
    {
        public static void Manipulate(Filter[] enumerable)
        {
            var forgeryTokenAttributes = enumerable.Select(f => f.Instance).OfType<ValidateAntiForgeryTokenAttribute>();
            foreach (var attr in forgeryTokenAttributes)
            {
                var validateActionProperty = typeof (ValidateAntiForgeryTokenAttribute).GetProperty("ValidateAction",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                validateActionProperty.SetValue(attr, new Action(() =>
                {
                    /*NOOP*/
                }));
            }
        }

        public static void Manipulate(ActionResult actionResult, RouteCollection routes)
        {
            var redirectResult = actionResult as RedirectToRouteResult;
            if (redirectResult != null)
            {
                var routesProperty = typeof(RedirectToRouteResult).GetProperty("Routes",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                routesProperty.SetValue(redirectResult, routes);
            }
        }
    }
}