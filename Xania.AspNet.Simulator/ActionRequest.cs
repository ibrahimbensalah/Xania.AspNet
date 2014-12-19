using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public abstract class ActionRequest : IActionRequest
    {
        public ActionRequest()
        {
            FilterProviders = new FilterProviderCollection(System.Web.Mvc.FilterProviders.Providers);
        }

        public FilterProviderCollection FilterProviders { get; private set; }

        public IPrincipal User { get; set; }

        public IValueProvider ValueProvider { get; set; }

        public ControllerBase Controller { get; set; }

        public ActionDescriptor ActionDescriptor { get; set; }

        public string HttpMethod { get; set; }

        public string UriPath { get; set; }

        public string HttpVersion { get; set; }

        public abstract IAction Action();

        public static ActionRequest Parse(String raw)
        {
            var lines = raw.Split('\n');
            var first = lines.First();

            var parts = first.Split(' ');
            var httpMethod = parts[0];
            var uriPath = parts[1];

            var httpVersion = parts[2];

            return new UrlActionRequest(uriPath, httpMethod)
            {
                HttpVersion = httpVersion
            };
        }
    }

    public class LinqActionRequest : ActionRequest
    {
        public override IAction Action()
        {
            return new ControllerAction(this, Controller, ActionDescriptor);
        }
    }
}