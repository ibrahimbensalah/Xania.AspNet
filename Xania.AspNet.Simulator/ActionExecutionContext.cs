using System.Web;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class ActionExecutionContext
    {
        public ControllerContext ControllerContext { get; set; }
        public ActionDescriptor ActionDescriptor { get; set; }

        public ControllerBase Controller
        {
            get { return ControllerContext.Controller; }
        }
        public dynamic ViewBag
        {
            get { return ControllerContext.Controller.ViewBag; }
        }
        public ViewDataDictionary ViewData
        {
            get { return ControllerContext.Controller.ViewData; }
        }

        public ModelStateDictionary ModelState
        {
            get { return ControllerContext.Controller.ViewData.ModelState; }
        }

        public HttpResponseBase Response
        {
            get { return ControllerContext.HttpContext.Response; }
        }

        public HttpRequestBase Request
        {
            get { return ControllerContext.HttpContext.Request; }
        }

    }
}