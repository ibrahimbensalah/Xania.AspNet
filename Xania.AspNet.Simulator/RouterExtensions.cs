using System;

namespace Xania.AspNet.Simulator
{
    public static class RouterExtensions
    {
        public static RouterAction Action(this Router router, string url)
        {
            return new RouterAction(router) { UriPath = url };
        }

        public static IControllerAction ParseAction(this Router router, string rawHttpRequest)
        {
            var action = new RouterAction(router);
            action.Raw(rawHttpRequest);

            return action;
        }
    }
}
