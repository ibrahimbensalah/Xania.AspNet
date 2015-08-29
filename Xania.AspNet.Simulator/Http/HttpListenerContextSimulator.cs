using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;

namespace Xania.AspNet.Simulator.Http
{
    internal class HttpListenerContextSimulator : HttpContextBase, IDisposable
    {
        private readonly HttpListenerContext _listenerContext;
        private readonly HttpSessionStateBase _session;
        private readonly HttpListenerResponseWrapper _response;
        private readonly HttpListenerRequestWrapper _request;
        private readonly Dictionary<object, object> _items;
        private readonly Cache _cache;
        private HttpApplication _applicationInstance;

        public HttpListenerContextSimulator(HttpListenerContext listenerContext, HttpSessionStateBase session)
        {
            _listenerContext = listenerContext;
            _session = session;
            _response = new HttpListenerResponseWrapper(listenerContext.Response, this);
            _request = new HttpListenerRequestWrapper(listenerContext.Request, () => this.User);
            _items = new Dictionary<object, object>();
            _cache = null;
            _applicationInstance = null;
        }

        public override HttpRequestBase Request
        {
            get { return _request; }
        }

        public override HttpResponseBase Response
        {
            get { return _response; }
        }

        public override HttpSessionStateBase Session
        {
            get { return _session; }
        }

        public override IDictionary Items
        {
            get { return _items; }
        }

        public override IPrincipal User { get; set; }

        public override Cache Cache
        {
            get { return _cache; }
        }

        public override HttpApplication ApplicationInstance
        {
            get { return _applicationInstance; }
            set { _applicationInstance = value; }
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
                _response.Dispose();
            }
        }

        ~HttpListenerContextSimulator()
        {
            Dispose(false);
        }
    }
}