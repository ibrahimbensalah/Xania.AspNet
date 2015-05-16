using System;
using System.IO;
using System.Security.Principal;
using System.Web;

namespace Xania.AspNet.Simulator
{
    public class AspNetUtility
    {
        internal static HttpContextBase GetContext(string url, string method, IPrincipal user)
        {
            var httpRequest = new SimpleHttpRequest
            {
                UriPath = url,
                User = user,
                HttpMethod = method
            };

            var worker = new ActionRequestWrapper(httpRequest);
            var httpContext = new HttpContext(worker)
            {
                User = httpRequest.User ?? CreateAnonymousUser()
            };

            return new HttpContextSimulator(httpContext);
        }

        public static IPrincipal CreateAnonymousUser()
        {
            return new GenericPrincipal(new GenericIdentity(String.Empty), new string[] { });
        }

    }

    internal class HttpRequestSimulator: HttpRequestWrapper
    {
        public HttpRequestSimulator(HttpRequest httpRequest) : base(httpRequest)
        {
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return "~" + Url.AbsolutePath; }
        }
    }
}