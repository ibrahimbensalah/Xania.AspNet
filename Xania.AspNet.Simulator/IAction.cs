using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public interface IAction
    {
        ControllerBase Controller { get; }

        ControllerActionResult Execute();

        FilterProviderCollection FilterProviders { get; }
    }
}