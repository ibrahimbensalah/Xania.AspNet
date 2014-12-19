using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public abstract class ActionRequest : IActionRequest
    {
        protected ActionRequest()
        {
            FilterProviders = new FilterProviderCollection(System.Web.Mvc.FilterProviders.Providers);
        }

        public FilterProviderCollection FilterProviders { get; private set; }

        public IPrincipal User { get; set; }

        public IValueProvider ValueProvider { get; set; }

        public string HttpMethod { get; set; }

        public string UriPath { get; set; }

        public string HttpVersion { get; set; }

        public static UrlActionRequest Parse(String raw)
        {
            var lines = raw.Split('\n');
            var first = lines.First();

            var parts = first.Split(' ');
            var httpMethod = parts[0];
            var uriPath = parts[1];

            var httpVersion = parts[2];

            return new UrlActionRequest(uriPath)
            {
                HttpVersion = httpVersion,
                HttpMethod = httpMethod
            };
        }
    }

    public class LinqActionRequest : ActionRequest
    {
        private readonly ControllerBase _controller;
        private readonly ActionDescriptor _actionDescriptor;

        public LinqActionRequest(ControllerBase controller, ActionDescriptor actionDescriptor)
        {
            _controller = controller;
            _actionDescriptor = actionDescriptor;
        }

        public ControllerBase Controller
        {
            get { return _controller; }
        }

        public ActionDescriptor ActionDescriptor
        {
            get { return _actionDescriptor; }
        }

        public IAction Action()
        {
            return new ControllerAction(this, Controller, ActionDescriptor);
        }
    }
}