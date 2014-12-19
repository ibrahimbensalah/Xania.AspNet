using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public class UrlActionRequest : ActionRequest
    {
        public UrlActionRequest(string url)
        {
            if (url.StartsWith("~"))
                url = url.Substring(1);

            UriPath = url;
            HttpVersion = "HTTP/1.1";
        }
    }
}