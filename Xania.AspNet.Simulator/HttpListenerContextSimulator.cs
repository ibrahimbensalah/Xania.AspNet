using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;

namespace Xania.AspNet.Simulator
{
    public class HttpListenerContextSimulator : HttpContextBase, IDisposable
    {
        private readonly HttpListenerContext _listenerContext;
        private readonly HttpListenerResponseWrapper _response;
        private readonly HttpListenerRequestWrapper _request;
        private readonly Dictionary<object, object> _items;
        private readonly Cache _cache;
        private HttpApplication _applicationInstance;

        public HttpListenerContextSimulator(HttpListenerContext listenerContext)
        {
            _listenerContext = listenerContext;
            _response = new HttpListenerResponseWrapper(listenerContext.Response);
            _request = new HttpListenerRequestWrapper(listenerContext.Request, this);
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