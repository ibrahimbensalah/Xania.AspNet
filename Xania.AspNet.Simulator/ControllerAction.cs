using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public abstract class ControllerAction: IHttpRequest, IControllerAction
    {
        protected ControllerAction(RouteCollection routes)
        {
            FilterProviders = new FilterProviderCollection(System.Web.Mvc.FilterProviders.Providers);
            Cookies = new Collection<HttpCookie>();
            Session = new Dictionary<string, object>();
            Files = new Dictionary<string, Stream>();
            Resolve = DependencyResolver.Current.GetService;
            Routes = routes;
        }

        public FilterProviderCollection FilterProviders { get; private set; }

        public IPrincipal User { get; set; }

        public RouteCollection Routes { get; private set; }

        public IValueProvider ValueProvider { get; set; }

        public IWebPageProvider WebPageProvider { get; set; }

        public ICollection<HttpCookie> Cookies { get; private set; }
        public IDictionary<string, object> Session { get; private set; }
        public IDictionary<string, Stream> Files { get; private set; }
        public Func<Type, object> Resolve { get; set; }

        public string HttpMethod { get; set; }

        public string UriPath { get; set; }

        public abstract ActionContext GetActionContext();

        protected virtual void Initialize(ControllerContext controllerContext)
        {
            HttpServerSimulator.PrintElapsedMilliseconds("initialize action");
            var controllerBase = controllerContext.Controller;

            // Use empty value provider by default to prevent use of ASP.NET MVC default value providers
            // Its not the purpose of this simulator framework to validate the ASP.NET MVC default value 
            // providers. Either a value provider is not need in case model values are predefined or a 
            // custom implementation is provided.
            var valueProviders = new ValueProviderCollection();
            if (ValueProvider != null)
                valueProviders.Add(ValueProvider);
            valueProviders.Add(new RouteDataValueProvider(controllerContext.RouteData, new CultureInfo("nl-NL")));
            controllerBase.ValueProvider = valueProviders;
            controllerBase.ControllerContext = controllerContext;
            var controller = controllerBase as Controller;
            if (controller != null)
            {
                controller.Url = new UrlHelper(controllerContext.RequestContext, Routes);

                IViewEngine viewEngine = new RazorViewEngineSimulator(WebPageProvider);
                controller.ViewEngineCollection = new ViewEngineCollection(new[] {viewEngine});
            }
            HttpServerSimulator.PrintElapsedMilliseconds("initialize action complete");
        }

        public virtual ControllerActionResult Execute()
        {
            var actionContext = GetActionContext();
            return Execute(actionContext.ControllerContext, actionContext.ActionDescriptor);
        }

        protected virtual ControllerActionResult Execute(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var invoker = GetActionInvoker(controllerContext, actionDescriptor);
            return new ControllerActionResult
            {
                ControllerContext = controllerContext,
                ActionResult = invoker.InvokeAction()
            };
        }

        internal virtual SimulatorActionInvoker GetActionInvoker(ControllerContext controllerContext,
            ActionDescriptor actionDescriptor)
        {
            
            var filters = FilterProviders.GetFilters(controllerContext, actionDescriptor).Select(BuildUp);

            return new SimulatorActionInvoker(controllerContext, actionDescriptor, filters);
        }

        protected virtual Filter BuildUp(Filter filter)
        {
            if (Resolve == null)
                return filter;

            foreach (PropertyDescriptor propertyDesc in TypeDescriptor.GetProperties(filter.Instance))
            {
                var typeNames = propertyDesc.Attributes.OfType<Attribute>().Select(e => e.GetType().ToString());
                if (typeNames.Contains("Microsoft.Practices.Unity.DependencyAttribute"))
                {
                    var service = Resolve(propertyDesc.PropertyType);
                    if (service != null)
                        propertyDesc.SetValue(filter.Instance, service);
                }
            }

            return filter;
        }

        public virtual ActionResult Authorize()
        {
            var actionContext = GetActionContext();
            return Authorize(actionContext.ControllerContext, actionContext.ActionDescriptor);
        }

        protected virtual ActionResult Authorize(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            Initialize(controllerContext);

            return GetActionInvoker(controllerContext, actionDescriptor).AuthorizeAction();
        }
    }

    public class MvcApplication: IMvcApplication
    {
        private readonly IControllerFactory _controllerFactory;
        private readonly IContentProvider _contentProvider;

        public MvcApplication(IControllerFactory controllerFactory, IContentProvider contentProvider = null)
        {
            _controllerFactory = controllerFactory;
            _contentProvider = contentProvider ?? GetDefaultContentProvider();

            Routes = GetRoutes();
        }

        public RouteCollection Routes { get; private set; }

        public static RouteCollection GetRoutes()
        {
            var routes = new RouteCollection(new ActionRouterPathProvider());

            if (RouteTable.Routes.Any())
                foreach (var r in RouteTable.Routes)
                    routes.Add(r);
            else
                routes.MapRoute(
                    "Default",
                    "{controller}/{action}/{id}",
                    new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                    );

            return routes;
        }


        private IContentProvider GetDefaultContentProvider()
        {
            var directories = new List<string>();

            var appDomainBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            directories.Add(appDomainBaseDirectory);

            var regex = new Regex(@"(.*)\\bin\\[^\\]*\\?$");

            var match = regex.Match(appDomainBaseDirectory);
            if (match.Success)
            {
                var sourceBaseDirectory = match.Groups[1].Value;
                directories.Add(sourceBaseDirectory);
            }

            return new DirectoryContentProvider(directories.ToArray());
        }

        public ControllerBase CreateController(string controllerName)
        {
            return _controllerFactory.CreateController(controllerName);
        }

        public Stream Open(string virtualPath)
        {
            return _contentProvider.Open(virtualPath);
        }

        public WebViewPageSimulator Create(ViewContext viewContext, string virtualPath)
        {
            var webViewPage = new WebViewPageFactory(this).Create(virtualPath);

            webViewPage.VirtualPath = virtualPath;
            webViewPage.ViewContext = viewContext;
            webViewPage.ViewData = viewContext.ViewData;

            webViewPage.Ajax = new AjaxHelper<object>(webViewPage.ViewContext, webViewPage, Routes);
            webViewPage.Html = new HtmlHelperSimulator<object>(viewContext, webViewPage, this);
            webViewPage.Url = new UrlHelper(webViewPage.ViewContext.RequestContext, Routes);

            return webViewPage;
        }
    }

    public class DirectoryContentProvider : IContentProvider
    {
        private readonly string[] _baseDirectories;

        public DirectoryContentProvider(params string[] baseDirectories)
        {
            _baseDirectories = baseDirectories;
        }

        public Stream Open(string virtualPath)
        {
            foreach (var baseDirectory in _baseDirectories)
            {
                var filePath = Path.Combine(baseDirectory, virtualPath);
                if (File.Exists(filePath))
                    return File.OpenRead(filePath);
            }

            throw new FileNotFoundException(String.Format("Path {0} not found in {1}", virtualPath, string.Join(",", _baseDirectories)));
        }
    }

    [Serializable]
    public class ControllerActionException : Exception
    {
        public ControllerActionException(string message)
            : base(message)
        {
        }
    }
}