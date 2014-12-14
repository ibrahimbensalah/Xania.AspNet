using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.Routing;
using Moq;

namespace Xania.AspNet.Simulator
{
    public class AspNetUtility
    {
        internal static RequestContext CreateRequestContext(string actionName, string controllerName, string httpMethod, IPrincipal user)
        {
            var httpContext = GetContext(String.Format("/{0}/{1}", controllerName, actionName), httpMethod, user);
            var routeData = new RouteData { Values = { { "controller", controllerName }, { "action", actionName } } };

            return new RequestContext(httpContext, routeData);
        }

        internal static HttpContextBase GetContext(String url, string method, IPrincipal user)
        {
            var worker = new MvcWorkerRequest("/", method);
            var httpContext = new HttpContext(worker);
            var x = httpContext.Request.RawUrl;
            return GetContext(httpContext, user);
        }

        internal static HttpContextBase GetContext(HttpContext httpContext, IPrincipal user)
        {
            var session = new Mock<HttpSessionStateBase>();

            // mock HttpRequest
            var requestBase = Wrap(httpContext.Request);

            // mock HttpResponse
            var responseBase = Wrap(httpContext.Response);

            // mock HttpContext
            return Wrap(requestBase, responseBase, session.Object, httpContext.Cache, user);
        }

        private static HttpContextBase Wrap(HttpRequestBase requestBase, HttpResponseBase responseBase, HttpSessionStateBase session, Cache cache, IPrincipal user)
        {
            var contextMock = new Mock<HttpContextBase>();
            contextMock.Setup(context => context.Items).Returns(new Hashtable());
            contextMock.Setup(context => context.Request).Returns(requestBase);
            contextMock.Setup(context => context.Response).Returns(responseBase);
            contextMock.Setup(context => context.Session).Returns(session);
            contextMock.Setup(context => context.Cache).Returns(cache);
            contextMock.Setup(context => context.User).Returns(user);
            return contextMock.Object;
        }

        private static HttpResponseBase Wrap(HttpResponse response)
        {
            var mock = new Mock<HttpResponseBase>();
            mock.Setup(wrapper => wrapper.StatusCode).Returns(response.StatusCode);
            mock.Setup(wrapper => wrapper.Output).Returns(response.Output);
            mock.Setup(wrapper => wrapper.Cache).Returns(new HttpCachePolicyWrapper(response.Cache));
            mock.Setup(wrapper => wrapper.Cookies).Returns(response.Cookies);

            return mock.Object;
        }

        private static HttpRequestBase Wrap(HttpRequest request)
        {
            Debug.Assert(request.Headers != null, "request.Headers != null");

            var mock = new Mock<HttpRequestBase>();
            mock.Setup(wrapper => wrapper.Url).Returns(request.Url);
            mock.Setup(wrapper => wrapper.HttpMethod).Returns(request.HttpMethod);
            mock.Setup(wrapper => wrapper.Form).Returns(request.Form);
            mock.Setup(wrapper => wrapper.ServerVariables).Returns(new NameValueCollection());
            mock.Setup(wrapper => wrapper.AppRelativeCurrentExecutionFilePath)
                .Returns("~" + request.Url.AbsolutePath);
            // requestMock.Setup(request => request.Unvalidated).Returns(unvalidatedRequest.Object);
            mock.Setup(wrapper => wrapper.ContentType).Returns(request.ContentType);
            mock.Setup(wrapper => wrapper.QueryString).Returns(request.QueryString);
            mock.Setup(wrapper => wrapper.Files).Returns(new Mock<HttpFileCollectionBase>().Object);
            mock.Setup(wrapper => wrapper.Headers).Returns(request.Headers);
            mock.Setup(wrapper => wrapper.Cookies).Returns(request.Cookies);

            return mock.Object;
        }
    }
}