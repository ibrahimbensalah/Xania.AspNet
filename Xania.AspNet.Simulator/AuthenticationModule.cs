using System;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace Xania.AspNet.Simulator
{
    public class AuthenticationModule: IServerModule
    {
        private readonly Func<IPrincipal> _userProvider;

        public AuthenticationModule(Func<IPrincipal> userProvider)
        {
            _userProvider = userProvider;
        }

        void IServerModule.Enter(HttpContextBase context)
        {
            context.User = _userProvider(); 
        }

        void IServerModule.Exit(HttpContextBase context)
        {
        }
    }
}