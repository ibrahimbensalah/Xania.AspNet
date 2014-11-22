using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public class MvcResult
    {
        public ControllerContext ControllerContext { get; set; }
        public ActionResult ActionResult { get; set; }

        public dynamic ViewBag
        {
            get { return this.ControllerContext.Controller.ViewBag; }
        }
    }
}