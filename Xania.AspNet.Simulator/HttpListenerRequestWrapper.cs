using System;
using System.Collections.Specialized;
using System.Net;
using System.Web;

namespace Xania.AspNet.Simulator
{
    internal class HttpListenerRequestWrapper: HttpRequestBase
    {
        private readonly HttpListenerRequest _request;
        private readonly string _physicalApplicationPath;
        private NameValueCollection _params;
        private NameValueCollection _serverVariables;
        private readonly HttpCookieCollection _cookies;

        public HttpListenerRequestWrapper(HttpListenerRequest request)
        {
            _request = request;
            _physicalApplicationPath = null;
            _cookies = new HttpCookieCollection();
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

        public override NameValueCollection ServerVariables
        {
            get
            {
                if (_serverVariables == null)
                {
                    _serverVariables = new NameValueCollection();
                }
                return _serverVariables;
            }
        }

        public override HttpCookieCollection Cookies
        {
            get { return _cookies; }
        }

        public override Uri Url
        {
            get { return _request.Url; }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return "~" + _request.RawUrl; }
        }

        public override string PhysicalApplicationPath
        {
            get { return _physicalApplicationPath; }
        }

        public override string FilePath
        {
            get { return _request.Url.AbsolutePath; }
        }

        public override string ApplicationPath
        {
            get
            {
                return "/";
            }
        }

        public override string PathInfo
        {
            get
            {
                return null;
            }
        }

        public override bool IsLocal
        {
            get { return true; }
        }

        public override bool IsAuthenticated
        {
            get { return _request.IsAuthenticated; }
        }

        public override string HttpMethod
        {
            get { return _request.HttpMethod; }
        }

        public override string MapPath(string virtualPath)
        {
            return @"C:\asdflaksdf\asdfa\asdfasdf.cshtml";
        }
    }
}