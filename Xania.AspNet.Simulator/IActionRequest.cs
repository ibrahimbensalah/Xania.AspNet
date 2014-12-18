using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public interface IActionRequest
    {
        string HttpMethod { get; set; }
        IPrincipal User { get; set; }

        ControllerContext CreateContext(ControllerBase controller, ActionDescriptor actionDescriptor,
            IValueProvider valueProvider);
    }
}