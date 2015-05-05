using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Xania.AspNet.Simulator
{
    public class HttpServerSimulator : IDisposable
    {
        private readonly HttpListener _listener;

        public HttpServerSimulator(params string[] prefixes)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            }

            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            _listener = new HttpListener();

            foreach (var prefix in prefixes)
            {
                _listener.Prefixes.Add(prefix);
            }
            _listener.Start();
        }

        public Task<HttpContextBase> GetContextAsync()
        {
            return
                _listener.GetContextAsync()
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                            return null;

                        return (HttpContextBase) new HttpListenerContextSimulator(task.Result);
                    });
        }

        public void Dispose()
        {
            _listener.Stop();
        }

        public async void Use(Action<HttpContextBase> worker)
        {
            bool running = true;

            while (running)
            {
                running = await GetContextAsync().ContinueWith(task =>
                {
                    var context = task.Result;
                    if (context == null)
                        return false;

                    try
                    {
                        worker(context);
                    }
                    finally
                    {
                        context.Response.Close();
                    }
                    return true;
                });
            }
        }
    }
}
