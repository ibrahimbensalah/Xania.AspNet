using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Xania.AspNet.Simulator
{
    public static class RouterExtensions
    {
        public static IAction Action(this Router router, string url, Action<ActionRequest> configure = null)
        {
            var requestInfo = new UrlActionRequest(url, "GET");
            if (configure != null)
                configure(requestInfo);

            return router.Action(requestInfo);
        }

        public static IAction ParseAction(this Router router, string rawHttpRequest)
        {
            var requestInfo = ActionRequest.Parse(rawHttpRequest);
            return router.Action(requestInfo);
        }
    }

    public class UrlActionRequest : ActionRequest
    {
        public UrlActionRequest(string url, string method)
        {
            if (url.StartsWith("~"))
                url = url.Substring(1);

            UriPath = url;
            HttpMethod = method;
            HttpVersion = "HTTP/1.1";
        }

        public override IAction Action()
        {
            return new ControllerAction(this, Controller, ActionDescriptor);
        }
    }
}
