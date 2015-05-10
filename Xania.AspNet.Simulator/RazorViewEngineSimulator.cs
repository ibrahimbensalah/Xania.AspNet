using System;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    internal class RazorViewEngineSimulator : IViewEngine
    {
        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            throw new NotImplementedException();
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            const string viewPath = @"C:\Development\GitHub\Xania.AspNet.Simulator\Xania.AspNet.Simulator.Tests\Server\Dummy.cshtml";
            var view = new RazorViewSimulator(viewPath, "~/Dummy.cshtml");

            return new ViewEngineResult(view, this);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
        }
    }
}