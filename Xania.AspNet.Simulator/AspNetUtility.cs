using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.Routing;
using System.Web.SessionState;
using Moq;

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
            var httpContext = new HttpContext(worker);
            
            var form = new NameValueCollection();
            foreach (var kvp in httpRequest.Form)
                form.Add(kvp.Key, kvp.Value);

            return GetContext(httpContext, httpRequest.User, form);
        }

        internal static HttpContextBase GetContext(HttpContext httpContext, IPrincipal user, NameValueCollection form)
        {
            // mock HttpRequest
            var requestBase = Wrap(httpContext.Request, form);

            // mock HttpResponse
            var responseBase = Wrap(httpContext.Response);

            // mock HttpContext
            return Wrap(requestBase, responseBase, httpContext.Cache, user);
        }

        private static HttpContextBase Wrap(HttpRequestBase requestBase, HttpResponseBase responseBase, Cache cache, IPrincipal user)
        {
            var contextMock = new Mock<HttpContextBase>();
            contextMock.Setup(context => context.Items).Returns(new Hashtable());
            contextMock.Setup(context => context.Request).Returns(requestBase);
            contextMock.Setup(context => context.Response).Returns(responseBase);
            contextMock.Setup(context => context.Session).Returns(new SimpleSessionState());
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

        private static HttpRequestBase Wrap(HttpRequest request, NameValueCollection form)
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
            mock.Setup(wrapper => wrapper.Form).Returns(form);

            return mock.Object;
        }
    }
}