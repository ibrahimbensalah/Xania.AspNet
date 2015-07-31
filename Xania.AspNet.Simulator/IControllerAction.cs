using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Xania.AspNet.Core;

namespace Xania.AspNet.Simulator
{
    public interface IControllerAction
    {
        string HttpMethod { get; set; }

        string UriPath { get; set; }

        IPrincipal User { get; set; }

        IMvcApplication MvcApplication { get; }
    }
}
