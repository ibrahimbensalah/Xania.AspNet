using System;
using System.Net;
using System.Security.Principal;
using System.Threading;

namespace Xania.AspNet.Simulator
{
    public class AuthenticationModule: IServerModule
    {
        private readonly Func<HttpListenerContext, IPrincipal> _userProvider;

        public AuthenticationModule(Func<HttpListenerContext, IPrincipal> userProvider)
        {
            _userProvider = userProvider;
        }

        void IServerModule.Enter(HttpListenerContext context)
        {
            Thread.CurrentPrincipal = _userProvider(context); 
        }

        void IServerModule.Exit(HttpListenerContext context)
        {
        }
    }
}