using System.Web.Mvc;
using System.Web.Routing;
using Xania.AspNet.Simulator.Razor;

namespace Xania.AspNet.Simulator
{
    public interface IMvcApplication : IControllerFactory, IContentProvider, IWebPageProvider
    {
        RouteCollection Routes { get; }
        object CreateInstance(string virtualPath);
    }
}