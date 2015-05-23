using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Core
{
    public interface IMvcApplication : IControllerFactory, IContentProvider, IWebPageProvider
    {
        RouteCollection Routes { get; }
        IWebViewPage Create(string virtualPath);

        IHtmlString Action(ViewContext viewContext, string actionName, object routeValues);
    }
}