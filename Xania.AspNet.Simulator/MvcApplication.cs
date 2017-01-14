using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Xml;
using Xania.AspNet.Core;
using Xania.AspNet.Razor;
using Xania.AspNet.Http;
using IControllerFactory = Xania.AspNet.Core.IControllerFactory;

namespace Xania.AspNet.Simulator
{
    public class MvcApplication : IMvcApplication
    {
        public MvcApplication(string appDir)
            : this(new DirectoryContentProvider(appDir))
        {
        }

        public MvcApplication(IContentProvider contentProvider)
            : this(new ControllerContainer(), contentProvider)
        {
        }

        public MvcApplication(IControllerFactory controllerFactory, IContentProvider contentProvider)
            : this(controllerFactory, contentProvider, GetRoutes(contentProvider))
        {
        }
        
        public MvcApplication(IControllerFactory controllerFactory, IContentProvider contentProvider, RouteCollection routes)
{
            if (controllerFactory == null)
                throw new ArgumentNullException("controllerFactory");
            if (contentProvider == null)
                throw new ArgumentNullException("contentProvider");

            ControllerFactory = controllerFactory;
            ContentProvider = contentProvider;

            Routes = routes;
            ViewEngines = new ViewEngineCollection()
            {
                new RazorViewEngineSimulator(this)
            };
            Bundles = new BundleCollection();
            FilterProviders = new FilterProviderCollection();
            foreach (var provider in System.Web.Mvc.FilterProviders.Providers)
            {
                FilterProviders.Add(provider);
            }

            ModelMetadataProvider = ModelMetadataProviders.Current;
            WebViewPageFactory = new WebViewPageFactory(Assemblies, GetNamespaces(this));

            Binders = new ModelBinderDictionary();
            foreach (var b in ModelBinders.Binders)
                Binders.Add(b);
        }

        public ModelBinderDictionary Binders { get; private set; }

        public ModelMetadataProvider ModelMetadataProvider { get; set; }

        public ViewEngineCollection ViewEngines { get; private set; }

        public IWebViewPageFactory WebViewPageFactory { get; private set; }

        public RouteCollection Routes { get; private set; }

        public BundleCollection Bundles { get; private set; }

        public FilterProviderCollection FilterProviders { get; private set; }

        public IValueProvider ValueProvider { get; set; }

        public IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            // Use empty value provider by default to prevent use of ASP.NET MVC default value providers
            // Its not the purpose of this simulator framework to validate the ASP.NET MVC default value 
            // providers. Either a value provider is not need in case model values are predefined or a 
            // custom implementation is provided.
            var valueProviders = new ValueProviderCollection();
            if (ValueProvider != null)
                valueProviders.Add(ValueProvider);
            valueProviders.Add(new SimulatorValueProvider(controllerContext, new CultureInfo("nl-NL")));

            var factory = new JsonValueProviderFactory();
            var jsonValueProvider = factory.GetValueProvider(controllerContext);
            if (jsonValueProvider != null)
                valueProviders.Add(jsonValueProvider);

            return valueProviders;
        }

        public IEnumerable<ModelValidationResult> ValidateModel(Type modelType, object modelValue, ControllerContext controllerContext)
        {
            var modelMetadata = ModelMetadataProvider.GetMetadataForType(() => modelValue, modelType);
            var validator = ModelValidator.GetModelValidator(modelMetadata, controllerContext);

            return validator.Validate(null);
        }

        public IVirtualDirectory GetVirtualDirectory(string virtualPath)
        {
            return new PhysicalVirtualDirectory(ContentProvider, virtualPath);
        }

        public IEnumerable<string> Assemblies
        {
            get
            {
                var result = new Dictionary<string, string>();
                AddAssembly<Uri>(result);
                AddAssembly<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>(result);
                AddAssembly<EnumerableQuery>(result);
                AddAssembly<CookieProtection>(result);

                if (ContentProvider.DirectoryExists("bin"))
                {
                    foreach (var assemblyPath in ContentProvider.GetFiles("bin\\*.dll"))
                    {
                        var i = assemblyPath.LastIndexOf('\\') + 1;
                        var fullName = assemblyPath.Substring(i);

                        result.Add(fullName, assemblyPath);
                    }
                }

                var runtimeAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .GroupBy(a => a.FullName)
                    .Select(grp => grp.First())
                    .Select(a => new { Name = a.GetName().Name + ".dll", a.Location })
                    .Where(a => !result.ContainsKey(a.Name) && !string.IsNullOrWhiteSpace(a.Location))
                    .ToArray();

                foreach (var assembly in runtimeAssemblies)
                {
                    if (!result.ContainsKey(assembly.Name))
                    {
                        result.Add(assembly.Name, assembly.Location);
                    }
                }

                return result.Values;
            }
        }

        private static void AddAssembly<T>(Dictionary<string, string> result)
        {
            result.Add(typeof(T).Assembly.GetName().Name + ".dll", typeof(T).Assembly.Location);
        }

        public IContentProvider ContentProvider { get; private set; }

        public IControllerFactory ControllerFactory { get; private set; }

        public static RouteCollection GetRoutes(IContentProvider contentProvider)
        {
            var routes = new RouteCollection(new ActionRouterPathProvider());

            if (RouteTable.Routes.Any())
                foreach (var r in RouteTable.Routes)
                    routes.Add(r);
            else
            {
                if (contentProvider.DirectoryExists("Areas"))
                {
                    foreach (var dir in contentProvider.GetDirectories("Areas/*"))
                    {
                        var areaName = new DirectoryInfo(dir).Name;
                        var route = routes.MapRoute(
                            areaName + "_default",
                            areaName + "/{controller}/{action}/{id}",
                            new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                            );
                        route.DataTokens["area"] = areaName;
                    }
                }

                routes.MapRoute(
                    "Default",
                    "{controller}/{action}/{id}",
                    new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                    );
            }

            return routes;
        }

        public Stream Open(string virtualPath)
        {
            var filePath = ToFilePath(virtualPath);
            return ContentProvider.Open(filePath);
        }

        private string ToFilePath(string virtualPath)
        {
            return virtualPath.Substring(2).Replace("/", "\\");
        }

        public bool FileExists(string virtualPath)
        {
            var relativePath = ToFilePath(virtualPath);
            return ContentProvider.FileExists(relativePath);
        }

        public string MapPath(string virtualPath)
        {
            var relativePath = ToFilePath(virtualPath);
            return ContentProvider.GetPhysicalPath(relativePath);
        }

        public string ToAbsoluteUrl(string path)
        {
            if (path.StartsWith("~"))
                return path.Substring(1);
            return path;
        }

        public IVirtualContent GetVirtualContent(string virtualPath)
        {
            return new FileVirtualContent(ContentProvider, virtualPath);
        }

        public string MapUrl(string filePath)
        {
            var relativePath = ContentProvider.GetRelativePath(filePath);
            return String.Format("/{0}", relativePath.Replace("\\", "/"));
        }

        public IHtmlString Action(ViewContext viewContext, string actionName, object routeValues)
        {
            var controllerName = viewContext.RouteData.GetRequiredString("controller");
            var controller = ControllerFactory.CreateController(viewContext.HttpContext, controllerName);

            var partialOutput = new StringWriter();

            var action = this.Action(controller, actionName)
                .RequestData(routeValues);

            action.HttpContext = new HttpContextDecorator(viewContext.HttpContext)
            {
                Response = {Output = partialOutput}
            };

            action.Execute();

            return MvcHtmlString.Create(partialOutput.ToString());
        }

        public static MvcApplication CreateDefault()
        {
            return new MvcApplication(new ControllerContainer(), new DirectoryContentProvider(AppDomain.CurrentDomain.BaseDirectory));
        }

        private static IEnumerable<string> GetNamespaces(IMvcApplication mvcApplication)
        {
            if (mvcApplication.FileExists("~/Views/Web.config"))
            {
                var virtualContent = mvcApplication.GetVirtualContent("~/Views/Web.config");
                var doc = new XmlDocument();
                using (var s = XmlReader.Create(virtualContent.Open()))
                {
                    doc.Load(s);

                    if (doc.DocumentElement != null)
                    {
                        var nodes = doc.DocumentElement
                            .SelectNodes("/configuration/system.web.webPages.razor/pages/namespaces/add");
                        if (nodes != null)
                            foreach (XmlNode n in nodes)
                            {
                                if (n.Attributes != null)
                                {
                                    var ns = n.Attributes["namespace"].Value;
                                    if (!ns.Equals("System.Web.Mvc.Html") && !ns.Equals("System.Web.Optimization"))
                                        yield return ns;
                                }
                            }
                    }
                }
                yield return "Xania.AspNet.Razor.Html";
            }
            else
            {
                var defaultList = new[]
                {
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.Web.Mvc",
                    "System.Web.Mvc.Ajax",
                    "Xania.AspNet.Razor.Html",
                    "System.Web.Routing"
                };

                foreach (var ns in defaultList)
                    yield return ns;
            }

            var auth = mvcApplication.Assemblies.Any(a => a.EndsWith("Microsoft.Web.WebPages.OAuth.dll"));
            if (auth)
            {
                yield return "DotNetOpenAuth.AspNet";
                yield return "Microsoft.Web.WebPages.OAuth";
            }
        }
    }
}
