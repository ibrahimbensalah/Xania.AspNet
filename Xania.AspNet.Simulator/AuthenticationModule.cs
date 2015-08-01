using System;
using System.Security.Principal;
using System.Web;

namespace Xania.AspNet.Simulator
{
    public class AuthenticationModule: IServerModule
    {
        private readonly Func<HttpContextBase, IPrincipal> _userProvider;

        public AuthenticationModule(Func<HttpContextBase, IPrincipal> userProvider)
        {
            _userProvider = userProvider;
        }

        void IServerModule.Enter(HttpContextBase context)
        {
            context.User = _userProvider(context); 
        }

        void IServerModule.Exit(HttpContextBase context)
        {
        }
    }
}