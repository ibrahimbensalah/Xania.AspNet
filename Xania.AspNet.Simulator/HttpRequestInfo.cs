using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Xania.AspNet.Simulator
{
    public class HttpRequestInfo
    {
        public HttpRequestInfo()
        {
            
        }

        public HttpRequestInfo(string url, string method)
        {
            if (url.StartsWith("~"))
                url = url.Substring(1);

            UriPath = url;
            HttpMethod = method;
            HttpVersion = "HTTP/1.1";
        }

        public string UriPath { get; set; }
        public string HttpMethod { get; set; }
        public string HttpVersion { get; set; }

        public IPrincipal User { get; set; }

        public IDictionary<string, object> Data { get; set; }

        public class Builder
        {
            private readonly HttpRequestInfo _requestInfo;

            public Builder(HttpRequestInfo requestInfo)
            {
                _requestInfo = requestInfo;
            }

            public Builder User(string userName, string[] roles, string identityType = "Simulator")
            {
                _requestInfo.User = new GenericPrincipal(new GenericIdentity(userName, identityType), roles ?? new string[] { });
                return this;
            }

            public Builder Post()
            {
                _requestInfo.HttpMethod = "POST";
                return this;
            }

            public Builder Data(object values)
            {
                _requestInfo.Data = values.ToDictionary();
                return this;
            }
        }

    }
}