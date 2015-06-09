using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
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


        public ControllerBase CreateController(string controllerName)
        {
            return ControllerFactory.CreateController(controllerName);
        }

        public Stream Open(string virtualPath)
        {
            var filePath = ToFilePath(virtualPath);
            return ContentProvider.Open(filePath);
        }

        public TextReader OpenText(string virtualPath, bool includeStartPage)
        {
            var relativePath = ToFilePath(virtualPath);
            var contentStream = ContentProvider.Open(relativePath);
            const string startPagePath = @"Views\_ViewStart.cshtml";

            return includeStartPage && !String.Equals(relativePath, startPagePath) &&
                   ContentProvider.Exists(startPagePath)
                ? (TextReader) new ConcatenatedStream(ContentProvider.Open(@"Views\_ViewStart.cshtml"), contentStream)
                : new StreamReader(contentStream);
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

        public string MapUrl(FileInfo file)
        {
            var relativePath = ContentProvider.GetRelativePath(file.FullName);
            return String.Format("/{0}", relativePath.Replace("\\", "/"));
        }

        public IHtmlString Action(ViewContext viewContext, string actionName, object routeValues)
        {
            var controllerName = viewContext.RouteData.GetRequiredString("controller");
            var action = ControllerFactory.Action(controllerName, actionName);
            action.Data(routeValues);

            action.Execute().ExecuteResult();
            return MvcHtmlString.Create(action.Output.ToString());

        }

    }

    internal class ConcatenatedStream : TextReader
    {
        private readonly IEnumerable<Stream> _streams;
        private readonly IEnumerator<StreamReader> _enumerator;

        public ConcatenatedStream(params Stream[] streams)
        {
            _streams = streams;
            _enumerator = _streams.Select(e => new StreamReader(e)).GetEnumerator();
            _enumerator.MoveNext();
        }

        public override int Read()
        {
            if (_enumerator.Current == null)
                return -1;

            var ch = _enumerator.Current.Read();
            if (ch != -1) 
                return ch;

            if (!_enumerator.MoveNext())
                return -1;

            return '\n';
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var stream in _streams)
                {
                    stream.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}