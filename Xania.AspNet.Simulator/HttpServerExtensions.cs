using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Xania.AspNet.Core;
using Xania.AspNet.Http;

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

            server.Use(httpContext =>
            {
                var httpContextBase = Wrap(httpContext, server.Sessions);
                var action = new HttpControllerAction(mvcApplication, httpContextBase);
                var executionContext = action.GetExecutionContext();

                if (executionContext != null)
                {
                    var actionResult = action.GetAuthorizationResult(executionContext);

                    if (actionResult == null)
                    {
                        action.ValidateRequest(executionContext);
                        actionResult = action.GetActionResult(executionContext);
                    }

                    actionResult.ExecuteResult(executionContext);

                    // close the response to enforce flush of the content
                    httpContextBase.Response.Close();

                    return true;
                }

                return false;
            });
        }

        private static HttpListenerContextSimulator Wrap(HttpListenerContext listenerContext, IDictionary<string, HttpSessionStateBase> sessions)
        {
            HttpSessionStateBase session;
            var sessionCookie = listenerContext.Request.Cookies["ASP.NET_SessionId"];
            if (sessionCookie == null)
            {
                session = new HttpSessionStateSimulator();
            }
            else if (!sessions.TryGetValue(sessionCookie.Value, out session))
            {
                session = new HttpSessionStateSimulator(sessionCookie.Value);
                sessions.Add(sessionCookie.Value, session);
            }

            return new HttpListenerContextSimulator(listenerContext, session);
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
