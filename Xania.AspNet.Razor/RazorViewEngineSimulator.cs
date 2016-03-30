using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
            var pathFormats = GetPathFormats(controllerContext.RouteData);

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

        private static string[] GetPathFormats(RouteData routeData)
        {
            var controllerName = routeData.GetRequiredString("controller");
            var areaName = GetAreaName(routeData);

            if (string.IsNullOrEmpty(areaName))
            {
                return new []
                {
                    String.Format(@"~/Views/{0}/{{0}}.cshtml", controllerName),
                    @"~/Views/Shared/{0}.cshtml"
                };
            }

            return new []
            {
                String.Format(@"~/Areas/{0}/Views/{1}/{{0}}.cshtml", areaName, controllerName),
                String.Format(@"~/Areas/{0}/Views/Shared/{{0}}.cshtml", areaName),
                @"~/Views/Shared/{0}.cshtml"
            };
        }

        private static string GetAreaName(RouteData routeData)
        {
            return routeData.DataTokens.ContainsKey("area") ? routeData.DataTokens["area"] as string: null;
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
        }
    }
}