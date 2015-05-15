using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    internal class RazorViewEngineSimulator : IViewEngine
    {
        private readonly RouteCollection _routes;
        private readonly IApplicationHostSimulator _applicationHost;

        public RazorViewEngineSimulator(IApplicationHostSimulator applicationHost, RouteCollection routes)
        {
            _routes = routes;
            _applicationHost = applicationHost;
            // _contentProvider = contentProvider ?? GetDefaultContentProvider();
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return FindView(controllerContext, partialViewName, null, useCache);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            HttpServerSimulator.PrintElapsedMilliseconds("findview started");
            var controllerName = controllerContext.RouteData.GetRequiredString("controller");
            var virtualPath = String.Format(@"Views\{0}\{1}.cshtml", controllerName, viewName);

            var view = new RazorViewSimulator(_applicationHost, virtualPath, _routes);

            HttpServerSimulator.PrintElapsedMilliseconds("findview completed");
            return new ViewEngineResult(view, this);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            var viewSimulator = view as IDisposable;
            if (viewSimulator != null)
                viewSimulator.Dispose();
        }
    }

    public class ControllerContextViewEngine : IViewEngine
    {
        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            var controller = controllerContext.Controller as Controller;
            if (controller == null)
                return null;

            return controller.ViewEngineCollection.FindPartialView(controllerContext, partialViewName);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            var controller = controllerContext.Controller as Controller;
            if (controller == null)
                return null;

            return controller.ViewEngineCollection.FindView(controllerContext, viewName, masterName);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
        }
    }

}