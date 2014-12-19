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
            var requestInfo = new ActionRequest(url, "GET");
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
}
