using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Xania.AspNet.Core;
using IControllerFactory = Xania.AspNet.Core.IControllerFactory;

namespace Xania.AspNet.Simulator
{
    public class MvcApplication : IMvcApplication
    {
        public MvcApplication([NotNull] Core.IControllerFactory controllerFactory, IContentProvider contentProvider)
        {
            if (controllerFactory == null) 
                throw new ArgumentNullException("controllerFactory");
            if (contentProvider == null) 
                throw new ArgumentNullException("contentProvider");

            ControllerFactory = controllerFactory;
            ContentProvider = contentProvider;

            Routes = GetRoutes();
            ViewEngines = new ViewEngineCollection();
            Bundles = new BundleCollection();
        }

        public ViewEngineCollection ViewEngines { get; private set; }

        public RouteCollection Routes { get; private set; }

        public BundleCollection Bundles { get; private set; }

        public IEnumerable<string> Assemblies
        {
            get
            {
                var result = new Dictionary<string, string>();
                AddAssembly<Uri>(result);
                AddAssembly<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>(result);
                AddAssembly<EnumerableQuery>(result);
                AddAssembly<CookieProtection>(result);

                foreach (var assemblyPath in ContentProvider.GetFiles("bin\\*.dll"))
                {
                    var i = assemblyPath.LastIndexOf('\\') + 1;
                    var fullName = assemblyPath.Substring(i);

                    result.Add(fullName, assemblyPath);
                }

                var runtimeAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .GroupBy(a => a.FullName)
                    .Select(grp => grp.First())
                    .Select(a => new { Name = a.GetName().Name + ".dll", a.Location})
                    .Where(a => !result.ContainsKey(a.Name) && !string.IsNullOrWhiteSpace(a.Location))
                    .ToArray();

                foreach (var assembly in runtimeAssemblies)
                {
                    result.Add(assembly.Name, assembly.Location);
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


        public ControllerBase CreateController(HttpContextBase context, string controllerName)
        {
            return ControllerFactory.CreateController(context, controllerName);
        }

        public Stream Open(string virtualPath)
        {
            var filePath = ToFilePath(virtualPath);
            return ContentProvider.Open(filePath);
        }

        public TextReader OpenText(string virtualPath, bool includeStartPage)
        {
            return ContentProvider.Open(ToFilePath(virtualPath), includeStartPage);
        }

        private string ToFilePath(string virtualPath)
        {
            return virtualPath.Substring(2).Replace("/", "\\");
        }

        public bool Exists(string virtualPath)
        {
            var relativePath = ToFilePath(virtualPath);
            return ContentProvider.Exists(relativePath);
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

        public string MapUrl(FileInfo file)
        {
            var relativePath = ContentProvider.GetRelativePath(file.FullName);
            return String.Format("/{0}", relativePath.Replace("\\", "/"));
        }

        public IHtmlString Action(ViewContext viewContext, string actionName, object routeValues)
        {
            const string parentActionViewContextToken = "ParentActionViewContext";

            var controllerName = viewContext.RouteData.GetRequiredString("controller");
            var controller = ControllerFactory.CreateController(viewContext.HttpContext, controllerName);

            var routeData = new RouteData
            {
                Values = {{"controller", controllerName}, {"action", actionName}},
                DataTokens = {{parentActionViewContextToken, viewContext}}
            };
            foreach (var kvp in new RouteValueDictionary(routeValues))
            {
                if (routeData.Values.ContainsKey(kvp.Key))
                    continue;

                routeData.Values.Add(kvp.Key, kvp.Value);
            }

            var httpContext = viewContext.HttpContext;
            var mainOutput = httpContext.Response.Output;
            try
            {
                var partialOutput = new StringWriter();
                httpContext.Response.Output = partialOutput;
               
                controller.ControllerContext = new ControllerContext(httpContext, routeData, controller);

                var action = controller.Action(this, actionName);
                action.Data(routeValues);

                action.Execute().ExecuteResult();

                return MvcHtmlString.Create(partialOutput.ToString());
            }
            finally
            {
                httpContext.Response.Output = mainOutput;
            }

        }
    }
}