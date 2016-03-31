using System.Collections.Generic;
using System.Net;
using System.Web;
using Xania.AspNet.Core;
using Xania.AspNet.Http;

namespace Xania.AspNet.Simulator
{
    public class MvcServerHandler : IHttpServerHandler
    {
        private readonly HttpServerSimulator _server;
        private readonly IMvcApplication _mvcApplication;

        public MvcServerHandler(HttpServerSimulator server, IMvcApplication mvcApplication)
        {
            _server = server;
            _mvcApplication = mvcApplication;
        }

        public bool Handle(HttpListenerContext httpContext)
        {
            var httpContextBase = Wrap(httpContext, _server.Sessions);

            _server.OnEnter(httpContextBase);

            var action = new HttpControllerAction(_mvcApplication, httpContextBase);
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
    }
}