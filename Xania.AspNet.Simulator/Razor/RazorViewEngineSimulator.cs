using System;
using System.Web.Mvc;
using Xania.AspNet.Simulator;

namespace Xania.AspNet.Simulator.Razor
{
    internal class RazorViewEngineSimulator : IViewEngine
    {
        private readonly IWebPageProvider _webPageProvider;

        public RazorViewEngineSimulator(IWebPageProvider webPageProvider)
        {
            _webPageProvider = webPageProvider;
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return FindView(controllerContext, partialViewName, null, useCache);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            HttpServerSimulator.PrintElapsedMilliseconds("findview started");
            var controllerName = controllerContext.RouteData.GetRequiredString("controller");
            var virtualPath = String.Format(@"~/Views/{0}/{1}.cshtml", controllerName, viewName);

            var view = new RazorViewSimulator(_webPageProvider, virtualPath);

            HttpServerSimulator.PrintElapsedMilliseconds("findview completed");
            return new ViewEngineResult(view, this);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
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