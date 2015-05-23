using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xania.AspNet.Core;

namespace Xania.AspNet.Simulator
{
    public abstract class ControllerAction: IHttpRequest, IControllerAction
    {
        protected ControllerAction(RouteCollection routes)
            : this(routes, new ViewEngineCollection())
        {
        }

        protected ControllerAction(RouteCollection routes, ViewEngineCollection viewEngines)
        {
            Routes = routes;
            ViewEngines = viewEngines;

            FilterProviders = new FilterProviderCollection(System.Web.Mvc.FilterProviders.Providers);
            Cookies = new Collection<HttpCookie>();
            Session = new Dictionary<string, object>();
            Files = new Dictionary<string, Stream>();
            Resolve = DependencyResolver.Current.GetService;
        }

        public FilterProviderCollection FilterProviders { get; private set; }

        public IPrincipal User { get; set; }

        public IValueProvider ValueProvider { get; set; }

        public ICollection<HttpCookie> Cookies { get; private set; }
        public IDictionary<string, object> Session { get; private set; }
        public IDictionary<string, Stream> Files { get; private set; }
        public Func<Type, object> Resolve { get; set; }

        public string HttpMethod { get; set; }

        public string UriPath { get; set; }

        public RouteCollection Routes { get; private set; }

        public ViewEngineCollection ViewEngines { get; private set; }

        public abstract ActionContext GetActionContext();

        protected virtual void Initialize(ControllerContext controllerContext)
        {
            HttpServerSimulator.PrintElapsedMilliseconds("initialize action");
            var controller = controllerContext.Controller as Controller;

            if (controller != null)
            {
                // Use empty value provider by default to prevent use of ASP.NET MVC default value providers
                // Its not the purpose of this simulator framework to validate the ASP.NET MVC default value 
                // providers. Either a value provider is not need in case model values are predefined or a 
                // custom implementation is provided.
                var valueProviders = new ValueProviderCollection();
                if (ValueProvider != null)
                    valueProviders.Add(ValueProvider);
                valueProviders.Add(new RouteDataValueProvider(controllerContext.RouteData, new CultureInfo("nl-NL")));

                controller.ValueProvider = valueProviders;
                controller.ControllerContext = controllerContext;
                controller.Url = new UrlHelper(controllerContext.RequestContext, Routes);
                controller.ViewEngineCollection = ViewEngines;
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

    public class DirectoryContentProvider : IContentProvider
    {
        private readonly string[] _baseDirectories;

        public DirectoryContentProvider(params string[] baseDirectories)
        {
            _baseDirectories = baseDirectories;
        }

        public bool Exists(string relativePath)
        {
            return _baseDirectories
                .Select(baseDirectory => Path.Combine(baseDirectory, relativePath))
                .Any(File.Exists);
        }

        public Stream Open(string relativePath)
        {
            foreach (var baseDirectory in _baseDirectories)
            {
                var filePath = Path.Combine(baseDirectory, relativePath);
                if (File.Exists(filePath))
                    return File.OpenRead(filePath);
            }

            throw new FileNotFoundException(String.Format("Path {0} not found in {1}", relativePath, string.Join(",", _baseDirectories)));
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