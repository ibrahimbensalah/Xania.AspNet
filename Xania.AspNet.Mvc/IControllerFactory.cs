using System.Web.Mvc;

namespace Xania.AspNet.Core
{
    public interface IControllerFactory
    {
        ControllerBase CreateController(string controllerName);
    }
}