using System;
using System.Linq;

namespace Xania.AspNet.Simulator
{
    public static class RouterExtensions
    {
        public static HttpControllerAction Action(this ControllerContainer controllerContainer, string url)
        {
            var mvcApplication = new MvcApplication(controllerContainer);
            return new HttpControllerAction(mvcApplication) { UriPath = url };
        }

        public static HttpControllerAction ParseAction(this ControllerContainer controllerContainer, string rawHttpRequest)
        {
            var lines = rawHttpRequest.Split('\n');
            var first = lines.First();

            var parts = first.Split(' ');
            return new HttpControllerAction(new MvcApplication(controllerContainer))
            {
                HttpMethod = parts[0],
                UriPath = parts[1]
            };
        }
    }
}
