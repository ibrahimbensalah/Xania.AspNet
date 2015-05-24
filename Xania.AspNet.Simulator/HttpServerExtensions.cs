using System;
using System.Web;
using System.Web.Mvc;
using Xania.AspNet.Core;

namespace Xania.AspNet.Simulator
{
    public static class HttpServerExtensions
    {
        public static IMvcApplication UseMvc(this HttpServerSimulator server, ControllerContainer controllerContainer)
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ControllerContextViewEngine());

            var mvcApplication = new MvcApplication(controllerContainer);

            server.Use(context =>
            {
                var action = new HttpControllerAction(mvcApplication, context)
                    .Execute();

                if (action != null)
                {
                    action.ExecuteResult();
                    return true;
                }

                return false;
            });

            return mvcApplication;
        }

    }
}
