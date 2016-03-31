using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Routing;

namespace Xania.AspNet.Http
{
    internal class HttpListenerContextSimulator : HttpContextBase, IDisposable
    {
        private readonly HttpSessionStateBase _session;
        private readonly HttpListenerResponseWrapper _response;
        private readonly HttpListenerRequestWrapper _request;
        private readonly Dictionary<object, object> _items;
        private readonly Cache _cache;
        private HttpApplication _applicationInstance;
        private IPrincipal _user;

        public HttpListenerContextSimulator(HttpListenerContext listenerContext, HttpSessionStateBase session)
        {
            _user = Thread.CurrentPrincipal;
            _session = session;
            _response = new HttpListenerResponseWrapper(listenerContext.Response, this);
            _request = new HttpListenerRequestWrapper(listenerContext.Request, new RequestContext(this, new RouteData()), () => User);
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

        public override IPrincipal User
        {
            get { return _user; }
            set { _user = value; }
        }

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