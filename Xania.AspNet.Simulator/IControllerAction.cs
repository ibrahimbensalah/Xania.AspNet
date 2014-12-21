using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public interface IControllerAction
    {
        ControllerActionResult Execute();
    }
}