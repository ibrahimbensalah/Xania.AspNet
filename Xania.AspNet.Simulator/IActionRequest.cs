using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public interface IActionRequest
    {
        string HttpMethod { get; set; }

        IPrincipal User { get; set; }

        ActionDescriptor ActionDescriptor { get; }

        FilterProviderCollection FilterProviders { get; }

        IValueProvider ValueProvider { get; }

        IAction Action();
    }
}