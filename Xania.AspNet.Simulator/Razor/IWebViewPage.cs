using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator.Razor
{
    public interface IWebViewPage
    {
        void Initialize(ViewContext viewContext, string virtualPath, IMvcApplication mvcApplication);
        void Execute(HttpContextBase httpContext, TextWriter writer);
    }
}