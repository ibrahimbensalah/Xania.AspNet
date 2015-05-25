using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Xania.AspNet.Core;

namespace Xania.AspNet.Simulator
{
    public static class HttpServerExtensions
    {

        public static IMvcApplication UseMvc(this HttpServerSimulator server, ControllerContainer controllerContainer, IContentProvider contentProvider = null)
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ControllerContextViewEngine());

            var mvcApplication = new MvcApplication(controllerContainer, contentProvider);

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

        public static void UseStatic(this HttpServerSimulator server, IContentProvider contentProvider)
        {
            contentProvider = contentProvider ?? DirectoryContentProvider.GetDefault();
            server.Use(context =>
            {
                var filePath = context.Request.FilePath.Substring(1);
                if (contentProvider.Exists(filePath))
                {
                    contentProvider.Open(filePath).CopyTo(context.Response.OutputStream);
                    return true;
                }

                return false;
            });
        }

    }
}
