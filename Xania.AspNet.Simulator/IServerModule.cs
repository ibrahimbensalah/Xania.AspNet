using System.Net;
using System.Web;

namespace Xania.AspNet.Simulator
{
    public interface IServerModule
    {
        void Enter(HttpContextBase context);
        void Exit(HttpContextBase context);
    }
}