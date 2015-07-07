using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;

namespace Xania.AspNet.Simulator
{
    public class HttpServerSimulator : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly List<Func<HttpContextBase, bool>> _handlers = new List<Func<HttpContextBase, bool>>();
        private bool _running;

        public HttpServerSimulator(params string[] prefixes)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            }

            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            Sessions = new Dictionary<string, HttpSessionStateBase>(StringComparer.InvariantCultureIgnoreCase);

            _listener = new HttpListener();

            foreach (var prefix in prefixes)
            {
                _listener.Prefixes.Add(prefix);
            }
            _listener.Start();
        }

        /// <summary>
        /// Session object stores states accross requests. There
        /// </summary>
        public IDictionary<string, HttpSessionStateBase> Sessions { get; private set; }

        public void AddSession(string sessionId, string paramName, object value)
        {
            HttpSessionStateBase session;
            if (!Sessions.TryGetValue(sessionId, out session))
            {
                session = new SimpleSessionState(sessionId);
                Sessions.Add(sessionId, session);
            }

            session[paramName] = value;
        }

        public Task<HttpContextBase> GetContextAsync()
        {
            return
                _listener.GetContextAsync()
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                            return null;

                        var listenerContext = task.Result;
                        listenerContext.Response.AppendHeader("Server", "Xania");

                        HttpSessionStateBase session = null;
                        var sessionCookie = listenerContext.Request.Cookies["ASP.NET_SessionId"];
                        if (sessionCookie == null)
                        {
                            session = new SimpleSessionState();
                        }
                        else if(!Sessions.TryGetValue(sessionCookie.Value, out session))
                        {
                            session = new SimpleSessionState(sessionCookie.Value);
                            Sessions.Add(sessionCookie.Value, session);
                        }

                        return (HttpContextBase)new HttpListenerContextSimulator(listenerContext, session);
                    });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _listener.Stop();
                _listener.Close();
            }
        }

        public void Use(Func<HttpContextBase, bool> handler)
        {
            _handlers.Add(handler);
            EnsureStarted();
        }

        private async void EnsureStarted()
        {
            if (_running)
                return;
            _running = true;

            while (_running)
            {
                _running = await GetContextAsync().ContinueWith(task =>
                {
                    var context = task.Result;
                    if (context == null)
                        return false;

                    try
                    {
                        OnEnter(context);

                        if (!_handlers.Any(h => h(context)))
                        {
                            // not served
                            context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                            context.Response.StatusDescription = "Resource not found";
                        }
                    }
                    catch (HttpException ex)
                    {
                        context.Response.StatusCode = ex.GetHttpCode();
                        context.Response.StatusDescription = ex.Message;
                        context.Response.Write(ex.Message);
                        var htmlErrorMessage = ex.GetHtmlErrorMessage();

                        if (htmlErrorMessage != null)
                        {
                            context.Response.Write("\n");
                            context.Response.Write(htmlErrorMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());

                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.StatusDescription = "Internal Server Error";
                        context.Response.Write(ex.Message);
                        context.Response.Write("\n");
                        context.Response.Write(ex.StackTrace);
                    }
                    finally
                    {
                        context.Response.Close();
                    }
                    return true;
                });
            }
        }

        private void OnEnter(HttpContextBase context)
        {
            var cookie = context.Request.Cookies["__AUTH"];
            if (cookie != null)
            {
                context.User = new GenericPrincipal(new GenericIdentity(cookie.Value, "simulator"), new string[0]);
            }
            else
            {
                context.User = new GenericPrincipal(new GenericIdentity(string.Empty, "simulator"), new string[0]);
            }
        }
    }
}
