using System;
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

            var mvcApplication = new MvcApplication(controllerContainer, contentProvider);
            
            UseMvc(server, mvcApplication);

            return mvcApplication;
        }

        public static void UseMvc(this HttpServerSimulator server, IMvcApplication mvcApplication)
        {
            SimulatorHelper.InitializeMembership();
            server.Use(new MvcServerHandler(server, mvcApplication));
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
                var filePath = context.Request.Url.AbsolutePath.Substring(1);
                if (contentProvider.FileExists(filePath))
                {
                    contentProvider.Open(filePath).CopyTo(context.Response.OutputStream);
                    return true;
                }

                return false;
            });
        }
    }
}
