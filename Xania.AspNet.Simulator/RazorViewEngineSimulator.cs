using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    internal class RazorViewEngineSimulator : IViewEngine
    {
        private readonly IContentProvider _contentProvider;

        public RazorViewEngineSimulator(IContentProvider contentProvider)
        {
            _contentProvider = contentProvider ?? GetDefaultContentProvider();
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

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return FindView(controllerContext, partialViewName, null, useCache);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            var controllerName = controllerContext.RouteData.GetRequiredString("controller");
            var virtualPath = String.Format(@"Views\{0}\{1}.cshtml", controllerName, viewName);
            var content = _contentProvider.Open(virtualPath);

            var view = new RazorViewSimulator(new WebViewPageFactory(_contentProvider), virtualPath);

            return new ViewEngineResult(view, this);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            var viewSimulator = view as IDisposable;
            if (viewSimulator != null)
                viewSimulator.Dispose();
        }
    }
}