using System;
using System.Web;
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
            return new ViewEngineResult(GetView(controllerContext, partialViewName, true), this);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return new ViewEngineResult(GetView(controllerContext, viewName, false), this);
        }

        private RazorViewSimulator GetView(ControllerContext controllerContext, string viewName, bool isPartial)
        {
            var virtualPath = GetVirtualPath(controllerContext, viewName);
            var virtualContent = _mvcApplication.GetVirtualContent(virtualPath);
            return new RazorViewSimulator(_mvcApplication, virtualContent, isPartial);
        }

        protected virtual string GetVirtualPath(ControllerContext controllerContext, string viewName)
        {
            var controllerName = controllerContext.RouteData.GetRequiredString("controller");

            string[] pathFormats =
            {
                String.Format(@"~/Views/{0}/{{0}}.cshtml", controllerName),
                @"~/Views/Shared/{0}.cshtml"
            };

            foreach (var pathFormat in pathFormats)
            {
                var virtualPath = String.Format(pathFormat, viewName);

                if (_mvcApplication.FileExists(virtualPath))
                {
                    return virtualPath;
                }
            }
            throw new HttpException(404, "View '" + viewName + "' not found");
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
        }
    }
}