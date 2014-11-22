using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace Xania.AspNet.Simulator
{
    public class AspNetUtility
    {
        internal static RequestContext CreateRequestContext<TController>(string actionName, IPrincipal user, Stream outputStream)
        {
            var controllerDescriptor = new ReflectedControllerDescriptor(typeof(TController));
            var controllerName = controllerDescriptor.ControllerName;

            return CreateRequestContext(actionName, controllerName, user, outputStream);
        }

        internal static RequestContext CreateRequestContext(string actionName, string controllerName, IPrincipal user, Stream outputStream)
        {
            var httpContext = GetContext(CreateHttpContext("~/controller/action", outputStream), user);
            var routeData = new RouteData { Values = { { "controller", controllerName }, { "action", actionName } } };

            return new RequestContext(httpContext, routeData);
        }

        internal static HttpContextBase GetContext(HttpContext httpContext, IPrincipal user)
        {
            var session = new Mock<HttpSessionStateBase>();

            // mock HttpRequest
            var requestBase = Wrap(httpContext.Request);

            // mock HttpResponse
            var responseBase = Wrap(httpContext.Response);

            return Wrap(requestBase, responseBase, session.Object, httpContext.Cache, user);
            // mock HttpContext
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

            return mock.Object;
        }

        private static HttpRequestBase Wrap(HttpRequest request)
        {
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

            return mock.Object;
        }

        //private static ControllerContext CreateControllerContext(RouteData routeData, ControllerBase controller)
        //{
        //    return new ControllerContext(new HttpContextWrapper(CreateHttpContext("~/")), routeData, controller);
        //}

        internal static HttpContext CreateHttpContext(string url, Stream outputStream = null)
        {
            var uri = new Uri("http://localhost/" + url.Substring(2));

            var request = new HttpRequest("", uri.ToString(), uri.Query)
            {
                Browser = new HttpBrowserCapabilities()
                {
                    Capabilities = new Dictionary<string, string>()
                }
            };

            var writer = new StreamWriter(outputStream ?? new MemoryStream());
            var response = new HttpResponse(writer);
            HttpContext.Current = new HttpContext(request, response)
            {
            };

            return HttpContext.Current;
        }

        internal static string GetActionName<TController>(Expression<Func<TController, object>> actionExpression)
        {
            var methodCallExpression = actionExpression.Body as MethodCallExpression;
            if (methodCallExpression == null)
                throw new ArgumentException("actionExpression is not a method call expression like: ctrl => ctrl.DoAction(...)");

            return methodCallExpression.Method.Name;
        }
    }
}