using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public class ActionContext
    {
        public ControllerContext ControllerContext { get; set; }
        public ActionDescriptor ActionDescriptor { get; set; }
    }
}