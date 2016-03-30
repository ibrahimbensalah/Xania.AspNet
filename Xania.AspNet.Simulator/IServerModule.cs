using System.Net;

namespace Xania.AspNet.Simulator
{
    public interface IServerModule
    {
        void Enter(HttpListenerContext context);
        void Exit(HttpListenerContext context);
    }
}