using System.Web.Mvc;

namespace Xania.AspNet.Core
{
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