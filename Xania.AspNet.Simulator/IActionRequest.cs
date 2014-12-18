using System.Security.Principal;

namespace Xania.AspNet.Simulator
{
    public interface IActionRequest
    {
        string HttpMethod { get; set; }
        IPrincipal User { get; set; }
    }
}