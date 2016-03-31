using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace Xania.AspNet.Http
{
    internal class HttpListenerResponseWrapper : HttpResponseBase, IDisposable
    {
        private readonly HttpListenerResponse _listenerResponse;
        private TextWriter _output;
        private readonly MemoryStream _outputStream;
        private bool _closed = false;
        private readonly HttpCookieCollection _cookies;
        private readonly HttpCachePolicyBase _cache;

        public HttpListenerResponseWrapper(HttpListenerResponse listenerResponse, HttpContextBase context)
        {
            _listenerResponse = listenerResponse;
            _outputStream = new MemoryStream();
            _output = new StreamWriter(_outputStream);
            _cookies = new HttpCookieCollection();
            _cache = new HttpCachePolicySimulator(context.Cache);
        }

        public override string ContentType { get; set; }

        public override Encoding ContentEncoding { get; set; }

        public override int StatusCode
        {
            get { return _listenerResponse.StatusCode; }
            set { _listenerResponse.StatusCode = value; }
        }

        public override HttpCookieCollection Cookies
        {
            get { return _cookies; }
        }

        public override string Status
        {
            get 
            {
                return this.StatusCode.ToString(NumberFormatInfo.InvariantInfo) + " " + this.StatusDescription;
            }
            set
            {
                int i = value.IndexOf(' ');
                StatusCode = Int32.Parse(value.Substring(0, i), CultureInfo.InvariantCulture);
                StatusDescription = value.Substring(i + 1);
            }
        }

        public override string StatusDescription
        {
            get { return _listenerResponse.StatusDescription; }
            set { _listenerResponse.StatusDescription = value; }
        }

        public override TextWriter Output
        {
            get { return _output; }
            set { _output = value; }
        }

        public override Stream OutputStream
        {
            get { return _outputStream; }
        }

        public override NameValueCollection Headers
        {
            get { return _listenerResponse.Headers; }
        }

        public override HttpCachePolicyBase Cache
        {
            get { return _cache; }
        }

        public override string ApplyAppPathModifier(string virtualPath)
        {
            return virtualPath;
        }

        public override void Flush()
        {
        }

        public override void Write(char ch)
        {
            Output.Write(ch);
        }

        public override void Write(string s)
        {
            Output.Write(s);
        }

        public override void Redirect(string url)
        {
            _listenerResponse.Redirect(url);
        }

        public override void Redirect(string url, bool endResponse)
        {
            _listenerResponse.Redirect(url);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Output.Write(buffer, index, count);
        }

        public override void Close()
        {
            if (!_closed)
            {
                FlushCookies();

                _closed = true;
                _output.Flush();

                var buffer = _outputStream.ToArray();
                _listenerResponse.ContentLength64 = buffer.Length;
                var output = _listenerResponse.OutputStream;
                output.Write(buffer, 0, buffer.Length);
            }
        }

        private void FlushCookies()
        {
            foreach (var cookie in from string cookieName in _cookies.Keys select _cookies[cookieName])
            {
                _listenerResponse.Cookies.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain)
                {
                    Expires = cookie.Expires
                });
            }
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
                _outputStream.Dispose();
                _output.Dispose();
            }
        }

        ~HttpListenerResponseWrapper()
        {
            Dispose(false);
        }
    }

    internal class HttpCachePolicySimulator : HttpCachePolicyBase
    {
        private readonly Cache _cache;
        private TimeSpan _delta;

        public HttpCachePolicySimulator(Cache cache)
        {
            _cache = cache;
        }

        public override void SetProxyMaxAge(TimeSpan delta)
        {
            _delta = delta;
        }

        public override void AddValidationCallback(HttpCacheValidateHandler handler, object data)
        {
        }
    }
}