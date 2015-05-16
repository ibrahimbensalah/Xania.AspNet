using System;
using System.Linq;

namespace Xania.AspNet.Simulator
{
    public static class RouterExtensions
    {
        public static HttpControllerAction Action(this ControllerContainer controllerContainer, string url)
        {
            return new HttpControllerAction(controllerContainer) { UriPath = url };
        }

        public static HttpControllerAction ParseAction(this ControllerContainer controllerContainer, string rawHttpRequest)
        {
            var lines = rawHttpRequest.Split('\n');
            var first = lines.First();

            var parts = first.Split(' ');
            return new HttpControllerAction(controllerContainer)
            {
                HttpMethod = parts[0],
                UriPath = parts[1]
            };
        }
    }
}
