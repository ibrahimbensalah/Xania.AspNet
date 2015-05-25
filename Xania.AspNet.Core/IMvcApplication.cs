using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Core
{
    public interface IMvcApplication : IControllerFactory, IContentProvider
    {
        RouteCollection Routes { get; }

        ViewEngineCollection ViewEngines { get; }

        IHtmlString Action(ViewContext viewContext, string actionName, object routeValues);

        TextReader OpenText(string virtualPath, bool includeStartPage);

        ICollection<Bundle> Bundles { get; }
    }

    public class Bundle
    {
        private readonly Func<HttpContextBase, IEnumerable<string>> _factory;

        public Bundle(string path, Func<HttpContextBase, IEnumerable<string>> factory)
        {
            Path = path;
            _factory = factory;
        }

        public string Path { get; private set; }

        public IEnumerable<string> GetItems(HttpContextBase context)
        {
            return _factory(context);
        }
    }
}