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
    }
}