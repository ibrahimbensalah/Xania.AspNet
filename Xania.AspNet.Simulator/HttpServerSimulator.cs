using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Xania.AspNet.Simulator.Http;

namespace Xania.AspNet.Simulator
{
    public class HttpServerSimulator : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly List<IServerModule> _modules = new List<IServerModule>();
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
                session = new HttpSessionStateSimulator(sessionId);
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
                            session = new HttpSessionStateSimulator();
                        }
                        else if(!Sessions.TryGetValue(sessionCookie.Value, out session))
                        {
                            session = new HttpSessionStateSimulator(sessionCookie.Value);
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

        public void AddModule(IServerModule module)
        {
            _modules.Add(module);
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

                        OnExit(context);
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
                        PrintToHtml(ex, context.Response.Output);
                    }
                    catch (Exception ex)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.StatusDescription = "Internal Server Error";
                        PrintToHtml(ex, context.Response.Output);
                        context.Response.Write("\n");
                        // context.Response.Write(ex.StackTrace);
                    }
                    finally
                    {
                        context.Response.Close();
                    }
                    return true;
                });
            }
        }

        private static void PrintToHtml(Exception ex, TextWriter output)
        {
            while (ex != null)
            {
                Debug.WriteLine(ex.ToString());
                output.Write("<div>");
                output.Write(ex.Message);
                output.Write("<p>");
                output.Write(ex.StackTrace);
                output.Write("</p>");
                output.Write("</div>");
                ex = ex.InnerException;
            }
        }

        protected virtual void OnEnter(HttpContextBase context)
        {
            foreach (var mod in _modules)
            {
                mod.Enter(context);
            }
        }

        protected virtual void OnExit(HttpContextBase context)
        {
            foreach (var mod in _modules)
            {
                mod.Exit(context);
            }
        }
    }
}
