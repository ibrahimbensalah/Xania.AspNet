using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public interface IAction
    {
        ControllerBase Controller { get; }

        void Authenticate(IPrincipal user);

        ControllerActionResult Execute();

        FilterProviderCollection FilterProviders { get; }
    }
}