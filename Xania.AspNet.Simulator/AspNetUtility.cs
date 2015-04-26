using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public class AspNetUtility
    {
        internal static RequestContext CreateRequestContext(string actionName, string controllerName, string httpMethod, IPrincipal user, IDictionary<string, string> formData)
        {
            var httpContext = GetContext(String.Format("/{0}/{1}", controllerName, actionName), httpMethod, user, formData);
            var routeData = new RouteData { Values = { { "controller", controllerName }, { "action", actionName } } };

            return new RequestContext(httpContext, routeData);
        }

        internal static HttpContextBase GetContext(string url, string method, IPrincipal user, IDictionary<string, string> form)
        {
            return GetContext(new SimpleHttpRequest
            {
                UriPath = url,
                User = user,
                HttpMethod = method,
                Form = form
            });
        }

        internal static HttpContextBase GetContext(IHttpRequest httpRequest)
        {
            var worker = new ActionRequestWrapper(httpRequest);
            var httpContext = new HttpContext(worker)
            {
                User = httpRequest.User
            };

            return new HttpContextSimulator(httpContext);
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

    internal class HttpContextSimulator : HttpContextWrapper
    {
        private readonly HttpRequestSimulator _request;
        private readonly HttpResponseWrapper _response;
        private readonly SimpleSessionState _session;

        public HttpContextSimulator(HttpContext httpContext) : base(httpContext)
        {
            _request = new HttpRequestSimulator(httpContext.Request);
            _response = new HttpResponseWrapper(httpContext.Response);
            _session = new SimpleSessionState();
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
    }
}