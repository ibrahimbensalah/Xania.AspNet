using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace Xania.AspNet.Simulator
{
    public class MvcSimulator
    {
        public MvcSimulator()
        {
            User = new GenericPrincipal(new GenericIdentity(String.Empty), new string[] {});
            DependencyResolverFactory = new DefaultDependencyResolverFactory();
        }

        public IDependencyResolverFactory DependencyResolverFactory { get; set; }

        public IPrincipal User { get; set; }

        public virtual IMvcRequest CreateRequest<TController>(Expression<Func<TController, object>> actionExpression)
            where TController : ControllerBase
        {
            // 1. request context
            var requestContext = AspNetUtility.CreateRequestContext<TController>(GetActionName(actionExpression), User, new MemoryStream());

            // 2. register the resolver
            var resolver = DependencyResolverFactory.Create(requestContext.HttpContext);

            // 3. create controller
            var controller = CreateController<TController>(resolver, requestContext);

            // 4. combine
            return CreateMvcRequest(controller.ControllerContext, actionExpression);
        }

        private T CreateController<T>(IDependencyResolver resolver, RequestContext requestContext)
            where T: ControllerBase
        {
            var controller = (T)resolver.GetService(typeof(T));
            controller.ControllerContext = new ControllerContext(requestContext, controller);
            return controller;
        }

        private static LinqRequest<TController> CreateMvcRequest<TController>(ControllerContext controllerContext, Expression<Func<TController, object>> actionExpression)
            where TController: ControllerBase 
        {
            return new LinqRequest<TController>(controllerContext, actionExpression);
        }

        private static string GetActionName<TController>(Expression<Func<TController, object>> actionExpression)
        {
            var methodCallExpression = actionExpression.Body as MethodCallExpression;
            if (methodCallExpression == null)
                throw new ArgumentException("actionExpression is not a method call expression like: ctrl => ctrl.DoAction(...)");

            return methodCallExpression.Method.Name;
        }


        private IDependencyResolver InitDependencyResolver(HttpContextBase inHttpContext)
        {
            return null;
            //var fakeContext = new FakeWebContext(inHttpContext);

            //ServiceLocator.Register(c => fakeContext.Session).As<IWebSession>();
            //ServiceLocator.Register(c => fakeContext.Cache).As<IWebCache>();
            //ServiceLocator.Register(c => fakeContext).As<IWebContext>();

            //var resolver = ServiceLocator.Build();
            //DependencyResolver.SetResolver(resolver);

            //return resolver;
        }

        public class ExecuteResult
        {
            public ActionResult ActionResult { get; set; }
            public Stream ContentStream { get; set; }
            public HttpResponseBase Response { get; set; }
        }

        public MvcSimulator Authenticate(string userName)
        {
            System.Threading.Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(userName, "generic"), new string[]{});
            return this;
        }
    }

    public class AspNetUtility
    {
        public static RequestContext CreateRequestContext<TController>(string actionName, IPrincipal user, Stream outputStream)
        {
            var controllerDescriptor = new ReflectedControllerDescriptor(typeof(TController));
            var controllerName = controllerDescriptor.ControllerName;
            var httpContext = GetContext("~/controller/action", user, outputStream);
            var routeData = new RouteData { Values = { { "controller", controllerName }, { "action", actionName } } };
            return new RequestContext(httpContext, routeData);
        }

        public static HttpContextBase GetContext(string path, IPrincipal user, Stream outputStream)
        {
            var httpContext = CreateHttpContext(path, outputStream);
            var session = new Mock<HttpSessionStateBase>();

            // mock HttpRequest
            var requestMock = new Mock<HttpRequestBase>();
            requestMock.Setup(request => request.Url).Returns(httpContext.Request.Url);
            requestMock.Setup(request => request.HttpMethod).Returns(httpContext.Request.HttpMethod);
            requestMock.Setup(request => request.Form).Returns(httpContext.Request.Form);
            requestMock.Setup(request => request.ServerVariables).Returns(new NameValueCollection());
            requestMock.Setup(request => request.AppRelativeCurrentExecutionFilePath)
                .Returns("~" + httpContext.Request.Url.AbsolutePath);
            // requestMock.Setup(request => request.Unvalidated).Returns(unvalidatedRequest.Object);
            requestMock.Setup(request => request.ContentType).Returns(httpContext.Request.ContentType);
            requestMock.Setup(request => request.QueryString).Returns(httpContext.Request.QueryString);
            requestMock.Setup(request => request.Files).Returns(new Mock<HttpFileCollectionBase>().Object);

            // mock HttpResponse
            var responseBase = new Mock<HttpResponseBase>();
            responseBase.Setup(context => context.StatusCode).Returns(httpContext.Response.StatusCode);
            responseBase.Setup(context => context.Output).Returns(httpContext.Response.Output);

            // mock HttpContext
            var contextMock = new Mock<HttpContextBase>();
            contextMock.Setup(context => context.Items).Returns(new Hashtable());
            contextMock.Setup(context => context.Request).Returns(requestMock.Object);
            contextMock.Setup(context => context.Response).Returns(responseBase.Object);
            contextMock.Setup(context => context.Session).Returns(session.Object);
            contextMock.Setup(context => context.Cache).Returns(httpContext.Cache);
            contextMock.Setup(context => context.User).Returns(user);
            return contextMock.Object;
        }

        //private static ControllerContext CreateControllerContext(RouteData routeData, ControllerBase controller)
        //{
        //    return new ControllerContext(new HttpContextWrapper(CreateHttpContext("~/")), routeData, controller);
        //}

        private static HttpContext CreateHttpContext(string url, Stream outputStream = null)
        {
            var uri = new Uri("http://localhost/" + url.Substring(2));

            var request = new HttpRequest("", uri.ToString(), uri.Query)
            {
                Browser = new HttpBrowserCapabilities()
                {
                    Capabilities = new Dictionary<String, String>()
                }
            };

            var writer = new StreamWriter(outputStream ?? new MemoryStream());
            var response = new HttpResponse(writer);
            HttpContext.Current = new HttpContext(request, response)
            {
            };

            return HttpContext.Current;
        }
    }
}
