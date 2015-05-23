using System.IO;
using System.Web.Mvc;
using Xania.AspNet.Core;

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
            var webPage = Create(viewContext);
            webPage.Execute(viewContext.HttpContext, writer);
        }

        private IWebViewPage Create(ViewContext viewContext)
        {
            using (var reader = _webPageProvider.OpenText(_virtualPath, true))
            {
                return _webPageProvider.Create(viewContext, _virtualPath, reader);
            }
        }
    }
}