using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Web;

namespace Xania.AspNet.Simulator.Http
{
    internal class HttpListenerRequestWrapper: HttpRequestBase
    {
        private readonly HttpListenerRequest _request;
        private readonly Func<IPrincipal> _principalFunc;
        private readonly string _physicalApplicationPath;
        private NameValueCollection _params;
        private NameValueCollection _serverVariables;
        private readonly HttpCookieCollection _cookies;
        private NameValueCollection _form;

        public HttpListenerRequestWrapper(HttpListenerRequest request, Func<IPrincipal> principalFunc)
        {
            _request = request;
            _principalFunc = principalFunc;
            _physicalApplicationPath = null;
            _cookies = new HttpCookieCollection();
            foreach (Cookie cookie in _request.Cookies)
            {
                Debug.Assert(cookie != null, "cookie != null");
                _cookies.Add(new HttpCookie(cookie.Name, cookie.Value)
                {
                    Path = cookie.Path,
                    Domain = cookie.Domain
                });
            }
        }

        public override NameValueCollection Headers
        {
            get { return _request.Headers; }
        }

        public override NameValueCollection Form
        {
            get
            {
                if (_form == null)
                {
                    using (var reader = new StreamReader(_request.InputStream))
                    {
                        _form = HttpUtility.ParseQueryString(reader.ReadToEnd());
                    }
                }
                return _form;
            }
        }

        public override NameValueCollection QueryString
        {
            get { return _request.QueryString; }
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
            get
            {
                var user = _principalFunc();
                return (user != null && user.Identity != null && user.Identity.IsAuthenticated);
            }
        }

        public override string HttpMethod
        {
            get { return _request.HttpMethod; }
        }

        public override string RawUrl
        {
            get { return _request.RawUrl; }
        }

        public override string MapPath(string virtualPath)
        {
            return @"C:\asdflaksdf\asdfa\asdfasdf.cshtml";
        }
    }
}