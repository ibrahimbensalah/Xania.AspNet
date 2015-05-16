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

            RenderView(viewContext.HttpContext, writer, webPage);
        }

        private void RenderView(HttpContextBase httpContext, TextWriter writer, WebPageBase webPage)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (webPage == null)
                throw new InvalidOperationException();

            try
            {
                webPage.ExecutePageHierarchy(
                    new WebPageContext(httpContext, (WebPageRenderingBase) null, (object) null), writer, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}