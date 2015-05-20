using System.IO;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator.Razor
{
    public interface IWebPageProvider
    {
        IWebViewPage Create(ViewContext viewContext, string virtualPath, TextReader reader);

        TextReader OpenText(string virtualPath, bool includeStartPage);
    }
}