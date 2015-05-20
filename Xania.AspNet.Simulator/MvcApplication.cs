using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using Xania.AspNet.Simulator.Razor;

namespace Xania.AspNet.Simulator
{
    public class MvcApplication : IMvcApplication
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
            return Open(virtualPath, false);
        }

        public Stream Open(string virtualPath, bool includeStartPage)
        {
            var relativePath = ToRelativePath(virtualPath);
            var contentStream = _contentProvider.Open(relativePath);
            const string startPagePath = @"Views\_ViewStart.cshtml";

            return includeStartPage && !String.Equals(relativePath, startPagePath) && _contentProvider.Exists(startPagePath)
                ? new ConcatenatedStream(_contentProvider.Open(@"Views\_ViewStart.cshtml"), contentStream)
                : contentStream;
        }

        public IWebViewPage Create(ViewContext viewContext, string virtualPath)
        {
            using (var stream = Open(virtualPath, true))
            {
                var webViewPage = new WebViewPageFactory().Create(virtualPath, stream);
                webViewPage.Initialize(viewContext, virtualPath, this);

                return webViewPage;
            }
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

        public object CreateInstance(string virtualPath)
        {
            using (var stream = Open(virtualPath))
            {
                return new WebViewPageFactory().Create(virtualPath, stream);
            }
        }
    }

    internal class ConcatenatedStream : Stream
    {
        private readonly IEnumerable<Stream> _streams;
        private readonly IEnumerator<Stream> _enumerator;

        public ConcatenatedStream(params Stream[] streams)
        {
            _streams = streams;
            _enumerator = _streams.GetEnumerator();
            _enumerator.MoveNext();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_enumerator.Current == null)
                return 0;

            var bytesRead = _enumerator.Current.Read(buffer, offset, count);
            if (bytesRead != 0) 
                return bytesRead;

            if (!_enumerator.MoveNext())
                return 0;

            bytesRead += Read(buffer, offset + bytesRead, count - bytesRead);
            return bytesRead;
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
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