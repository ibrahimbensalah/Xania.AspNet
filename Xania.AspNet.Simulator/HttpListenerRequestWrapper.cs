using System;
using System.Collections.Specialized;
using System.Net;
using System.Web;

namespace Xania.AspNet.Simulator
{
    internal class HttpListenerRequestWrapper: HttpRequestBase
    {
        private readonly HttpListenerRequest _request;
        private NameValueCollection _params;

        public HttpListenerRequestWrapper(HttpListenerRequest request)
        {
            _request = request;
        }

        public override NameValueCollection Params
        {
            get
            {
                if (_params == null)
                {
                    _params = new NameValueCollection(_request.QueryString);
                }
                return _params;
            }
        }

        public override Uri Url
        {
            get { return _request.Url; }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return "~" + _request.RawUrl; }
        }

        public override string PathInfo
        {
            get
            {
                return null;
            }
        }
    }
}