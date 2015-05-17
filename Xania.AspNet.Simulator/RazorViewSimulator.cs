using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Xania.AspNet.Simulator
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