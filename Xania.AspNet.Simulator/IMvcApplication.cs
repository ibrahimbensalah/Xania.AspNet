using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;

namespace Xania.AspNet.Simulator
{
    public interface IMvcApplication : IControllerFactory, IContentProvider, IWebPageProvider, IVirtualPathFactory
    {
        RouteCollection Routes { get; }
    }

    public interface IWebPageProvider
    {
        IWebViewPage Create(ViewContext viewContext, string virtualPath);
    }
}