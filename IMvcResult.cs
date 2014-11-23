using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class ControllerActionResult
    {
        public ControllerContext ControllerContext { get; set; }
        public ActionResult ActionResult { get; set; }

        public dynamic ViewBag
        {
            get { return ControllerContext.Controller.ViewBag; }
        }
    }
}