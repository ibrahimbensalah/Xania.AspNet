using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Xania.AspNet.Http;

namespace Xania.AspNet.Simulator
{
    public class HttpServerSimulator : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly List<IServerModule> _modules = new List<IServerModule>();
        private readonly List<IHttpServerHandler> _handlers = new List<IHttpServerHandler>();
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

        public Task<HttpListenerContext> GetContextAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var listenerContext = _listener.GetContext();
                    listenerContext.Response.AppendHeader("Server", "Xania");

                    return listenerContext;
                }
                catch(HttpListenerException)
                {
                    return null;
                }
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

        public void Use(Func<HttpListenerContext, bool> handler)
        {
            Use(new FuncServerHandler(handler));
        }

        public void Use(IHttpServerHandler handler)
        {
            _handlers.Add(handler);
            EnsureStarted();
        }

        private void EnsureStarted()
        {
            if (_running)
                return;
            _running = true;

            Task.Factory.StartNew(() =>
            {
                while (_running)
                {
                    var getContext = GetContextAsync().ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                            return false;

                        var context = task.Result;

                        if (context == null)
                            return false;

                        try
                        {
                            if (!_handlers.Any(h => h.Handle(context)))
                            {
                                // not served
                                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                context.Response.StatusDescription = "Resource not found";
                            }
                        }
                        catch (HttpException ex)
                        {
                            var writer = new StreamWriter(context.Response.OutputStream, context.Response.ContentEncoding);

                            context.Response.StatusCode = ex.GetHttpCode();
                            context.Response.StatusDescription = ex.Message;

                            var htmlErrorMessage = ex.GetHtmlErrorMessage();

                            if (htmlErrorMessage != null)
                            {
                                writer.Write("\n");
                                writer.Write(htmlErrorMessage);
                            }
                            PrintToHtml(ex, writer);
                            writer.Flush();
                        }
                        catch (Exception ex)
                        {
                            var writer = new StreamWriter(context.Response.OutputStream, context.Response.ContentEncoding);

                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.StatusDescription = "Internal Server Error";
                            PrintToHtml(ex, writer);
                            writer.Write("\n");
                            writer.Write(ex.StackTrace);
                            writer.Flush();
                        }
                        finally
                        {
                            context.Response.Close();
                        }
                        return true;
                    });

                    _running = getContext.Result;
                }
            });
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

        internal virtual void OnEnter(HttpContextBase context)
        {
            foreach (var mod in _modules)
            {
                mod.Enter(context);
            }
        }

        internal virtual void OnExit(HttpContextBase context)
        {
            foreach (var mod in _modules)
            {
                mod.Exit(context);
            }
        }
    }

    public interface IHttpServerHandler
    {
        bool Handle(HttpListenerContext context);
    }

    public class FuncServerHandler : IHttpServerHandler
    {
        private readonly Func<HttpListenerContext, bool> _handler;

        public FuncServerHandler(Func<HttpListenerContext, bool> handler)
        {
            _handler = handler;
        }

        public bool Handle(HttpListenerContext context)
        {
            return _handler(context);
        }
    }
}
