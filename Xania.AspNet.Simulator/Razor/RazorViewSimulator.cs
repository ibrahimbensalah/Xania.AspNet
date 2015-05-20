using System.IO;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator.Razor
{
    internal class RazorViewSimulator : IView
    {
        private readonly IWebPageProvider _webPageProvider;
        private readonly string _virtualPath;

        public RazorViewSimulator(IWebPageProvider webPageProvider, string virtualPath)
        {
            _webPageProvider = webPageProvider;
            _virtualPath = virtualPath;
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var webPage = _webPageProvider.Create(viewContext, _virtualPath);

            webPage.Execute(viewContext.HttpContext, writer);
        }
    }
}