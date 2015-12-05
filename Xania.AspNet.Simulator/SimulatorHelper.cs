using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Xania.AspNet.Simulator
{
    static internal class SimulatorHelper
    {
        public static void InitializeFilters(Filter[] enumerable)
        {
            var forgeryTokenAttributes = enumerable.Select(f => f.Instance).OfType<ValidateAntiForgeryTokenAttribute>();
            foreach (var attr in forgeryTokenAttributes)
            {
                var validateActionProperty = typeof (ValidateAntiForgeryTokenAttribute).GetProperty("ValidateAction",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                var noop = new Action(() =>
                {
                    /*NOOP*/
                });

                validateActionProperty.SetValue(attr, noop, null);
            }
        }

        public static void InitizializeActionResults(ActionResult actionResult, RouteCollection routes)
        {
            var redirectResult = actionResult as RedirectToRouteResult;
            if (redirectResult != null)
            {
                var routesProperty = typeof(RedirectToRouteResult).GetProperty("Routes",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                routesProperty.SetValue(redirectResult, routes, null);
            }
        }

        public static void InitializeMembership()
        {
            var initializedProperty = typeof (Membership).GetField("s_Initialized",
                BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(initializedProperty != null, "initializedProperty != null");
            initializedProperty.SetValue(null, true);

            var initializedDefaultProviderProperty = typeof(Membership).GetField("s_InitializedDefaultProvider",
                BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(initializedDefaultProviderProperty != null, "initializedDefaultProviderProperty != null");
            initializedDefaultProviderProperty.SetValue(null, true);

            var providerProperty = typeof (Membership).GetField("s_Provider",
                BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(providerProperty != null, "providerProperty != null");
            // providerProperty.SetValue(null, new Pro());
        }
    }
}