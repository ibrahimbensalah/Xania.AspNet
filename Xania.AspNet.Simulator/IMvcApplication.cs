using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public interface IMvcApplication : IControllerFactory, IContentProvider, IWebPageProvider
    {
        RouteCollection Routes { get; }
    }

    public interface IWebPageProvider
    {
        WebViewPageSimulator Create(ViewContext viewContext, string virtualPath);
    }
}