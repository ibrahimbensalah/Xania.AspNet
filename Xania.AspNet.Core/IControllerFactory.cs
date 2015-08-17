using System.Web;
using System.Web.Mvc;

namespace Xania.AspNet.Core
{
    public interface IControllerFactory
    {
        ControllerBase CreateController(HttpContextBase context, string controllerName);
    }
}