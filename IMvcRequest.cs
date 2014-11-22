using System;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public interface IMvcRequest
    {
        void Authenticate(IPrincipal user);

        MvcResult Execute();
    }
}