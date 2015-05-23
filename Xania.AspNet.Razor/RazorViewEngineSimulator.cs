using System;
using System.Web.Mvc;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public class RazorViewEngineSimulator : IViewEngine
    {
        private readonly IMvcApplication _mvcApplication;

        public RazorViewEngineSimulator(IMvcApplication mvcApplication)
        {
            _mvcApplication = mvcApplication;
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return FindView(controllerContext, partialViewName, null, useCache);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            var controllerName = controllerContext.RouteData.GetRequiredString("controller");
            var virtualPath = String.Format(@"~/Views/{0}/{1}.cshtml", controllerName, viewName);

            var view = new RazorViewSimulator(_mvcApplication, virtualPath);

            return new ViewEngineResult(view, this);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
        }
    }
}