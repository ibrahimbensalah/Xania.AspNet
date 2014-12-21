using System;

namespace Xania.AspNet.Simulator
{
    public static class RouterExtensions
    {
        public static IAction Action(this Router router, string url, Action<ActionRequest> configure = null)
        {
            var actionRequest = new ActionRequest() { UriPath = url };

            if (configure != null)
                configure(actionRequest);

            return new RouterAction(actionRequest, router);
        }

        public static IAction ParseAction(this Router router, string rawHttpRequest)
        {
            var requestInfo = ActionRequest.Parse(rawHttpRequest);
            return new RouterAction(requestInfo, router);
        }
    }
}
