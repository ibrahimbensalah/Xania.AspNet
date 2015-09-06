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
            var virtualPath = GetVirtualPath(controllerContext, partialViewName);
            var virtualContent = _mvcApplication.GetVirtualContent(virtualPath);
            var view = new RazorViewSimulator(_mvcApplication, virtualContent, true);

            return new ViewEngineResult(view, this);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            var virtualPath = GetVirtualPath(controllerContext, viewName);
            var virtualContent = _mvcApplication.GetVirtualContent(virtualPath);
            var view = new RazorViewSimulator(_mvcApplication, virtualContent, false);

            return new ViewEngineResult(view, this);
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

                if (_mvcApplication.Exists(virtualPath))
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