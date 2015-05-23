using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xania.AspNet.Core;

namespace Xania.AspNet.Simulator
{
    public class MvcApplication : IMvcApplication
    {
        private readonly Core.IControllerFactory _controllerFactory;
        private readonly IContentProvider _contentProvider;

        public MvcApplication(Core.IControllerFactory controllerFactory, IContentProvider contentProvider = null)
        {
            _controllerFactory = controllerFactory;
            _contentProvider = contentProvider ?? GetDefaultContentProvider();

            Routes = GetRoutes();
            ViewEngines = new ViewEngineCollection();
        }

        public ViewEngineCollection ViewEngines { get; private set; }

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


        private Core.IContentProvider GetDefaultContentProvider()
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
            var relativePath = ToRelativePath(virtualPath);
            return _contentProvider.Open(relativePath);
        }

        public TextReader OpenText(string virtualPath, bool includeStartPage)
        {
            var relativePath = ToRelativePath(virtualPath);
            var contentStream = _contentProvider.Open(relativePath);
            const string startPagePath = @"Views\_ViewStart.cshtml";

            return includeStartPage && !String.Equals(relativePath, startPagePath) &&
                   _contentProvider.Exists(startPagePath)
                ? (TextReader) new ConcatenatedStream(_contentProvider.Open(@"Views\_ViewStart.cshtml"), contentStream)
                : new StreamReader(contentStream);
        }
        
        private string ToRelativePath(string virtualPath)
        {
            return virtualPath.Substring(2).Replace("/", "\\");
        }

        public bool Exists(string virtualPath)
        {
            var relativePath = ToRelativePath(virtualPath);
            return _contentProvider.Exists(relativePath);
        }

        public IHtmlString Action(ViewContext viewContext, string actionName, object routeValues)
        {
            var controllerName = viewContext.RouteData.GetRequiredString("controller");
            var action = this.Action(controllerName, actionName);
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