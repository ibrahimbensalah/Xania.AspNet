using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public interface IAction
    {
        ControllerActionResult Execute();
    }
}