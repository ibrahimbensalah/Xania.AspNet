using System.Linq;
using System.Web;

namespace Xania.AspNet.Simulator
{
    public static class RouterExtensions
    {
        public static RouterAction Action(this ControllerContainer controllerContainer, string url)
        {
            return new RouterAction(controllerContainer) { UriPath = url };
        }

        public static RouterAction ParseAction(this ControllerContainer controllerContainer, string rawHttpRequest)
        {
            var lines = rawHttpRequest.Split('\n');
            var first = lines.First();

            var parts = first.Split(' ');
            return new RouterAction(controllerContainer)
            {
                HttpMethod = parts[0],
                UriPath = parts[1]
            };
        }
    }
}
