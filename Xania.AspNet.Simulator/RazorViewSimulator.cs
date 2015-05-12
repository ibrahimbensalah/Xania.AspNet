using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Razor;
using System.Web.WebPages;
using Microsoft.CSharp;

namespace Xania.AspNet.Simulator
{
    internal class RazorViewSimulator : IView
    {
        private readonly WebViewPageFactory _webViewPageFactory;
        private readonly string _virtualPath;

        public RazorViewSimulator(WebViewPageFactory webViewPageFactory, string virtualPath)
        {
            _webViewPageFactory = webViewPageFactory;
            _virtualPath = virtualPath;
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var webPage = _webViewPageFactory.Create(_virtualPath);

            RenderView(viewContext, writer, webPage);
        }

        private void RenderView(ViewContext viewContext, TextWriter writer, object instance)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            var webViewPage = instance as WebViewPage;
            if (webViewPage == null)
                throw new InvalidOperationException();

            webViewPage.VirtualPath = _virtualPath;
            webViewPage.ViewContext = viewContext;
            webViewPage.ViewData = viewContext.ViewData;
            webViewPage.InitHelpers();

            webViewPage.ExecutePageHierarchy(new WebPageContext(viewContext.HttpContext, (WebPageRenderingBase)null, (object)null), writer, null);
        }
    }
}