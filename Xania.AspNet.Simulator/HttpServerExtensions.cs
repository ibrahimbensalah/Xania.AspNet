﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Xania.AspNet.Core;

namespace Xania.AspNet.Simulator
{
    public static class HttpServerExtensions
    {

        public static IMvcApplication UseMvc(this HttpServerSimulator server, ControllerContainer controllerContainer)
        {
            return UseMvc(server, controllerContainer, new EmptyContentProvider());
        }

        public static IMvcApplication UseMvc(this HttpServerSimulator server, ControllerContainer controllerContainer,
            string appPath)
        {
            return UseMvc(server, controllerContainer, new DirectoryContentProvider(appPath));
        }

        public static IMvcApplication UseMvc(this HttpServerSimulator server, ControllerContainer controllerContainer, IContentProvider contentProvider)
        {
            if (controllerContainer == null) 
                throw new ArgumentNullException("controllerContainer");
            if (contentProvider == null) 
                throw new ArgumentNullException("contentProvider");

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ControllerContextViewEngine());

            var mvcApplication = new MvcApplication(controllerContainer, contentProvider);
            
            UseMvc(server, mvcApplication);

            return mvcApplication;
        }

        private static void UseMvc(HttpServerSimulator server, IMvcApplication mvcApplication)
        {
            server.Use(context =>
            {
                var action = new HttpControllerAction(mvcApplication, context)
                    .Execute();

                if (action != null)
                {
                    // Content-Type: text/html; charset=utf-8
                    context.Response.Headers.Add("Content-Type", "text/html; charset=utf-8");
                    context.Response.Headers.Add("Cache-Control", "private");

                    action.ExecuteResult();
                    return true;
                }

                return false;
            });
        }

        public static void UseStatic(this HttpServerSimulator server, string path)
        {
            UseStatic(server, new DirectoryContentProvider(path));
        }

        public static void UseStatic(this HttpServerSimulator server, IContentProvider contentProvider)
        {
            if (contentProvider == null) 
                throw new ArgumentNullException("contentProvider");

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
