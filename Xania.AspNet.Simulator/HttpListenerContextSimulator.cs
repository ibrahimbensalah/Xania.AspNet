using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace Xania.AspNet.Simulator
{
    public class HttpListenerContextSimulator : HttpContextBase, IDisposable
    {
        private readonly HttpListenerResponseWrapper _response;
        private readonly HttpListenerRequestWrapper _request;
        private readonly Dictionary<object, object> _items;

        public HttpListenerContextSimulator(HttpListenerContext listenerContext)
        {
            _response = new HttpListenerResponseWrapper(listenerContext.Response);
            _request = new HttpListenerRequestWrapper(listenerContext.Request);
            _items = new Dictionary<object, object>();
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