using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Razor;
using System.Web.Routing;
using System.Web.WebPages;
using Microsoft.CSharp;

namespace Xania.AspNet.Simulator
{
    internal class RazorViewSimulator : IView
    {
        private readonly IApplicationHostSimulator _applicationHost;
        private readonly string _virtualPath;
        private readonly RouteCollection _routes;

        public RazorViewSimulator(IApplicationHostSimulator applicationHost, string virtualPath, RouteCollection routes)
        {
            _applicationHost = applicationHost;
            _virtualPath = virtualPath;
            _routes = routes;
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var webPage = _applicationHost.Create(_virtualPath);

            RenderView(viewContext, writer, webPage);
        }

        private void RenderView(ViewContext viewContext, TextWriter writer, WebViewPageSimulator webViewPage)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (webViewPage == null)
                throw new InvalidOperationException();

            webViewPage.VirtualPath = _virtualPath;
            webViewPage.ViewContext = viewContext;
            webViewPage.ViewData = viewContext.ViewData;

            webViewPage.Ajax = new AjaxHelper<object>(webViewPage.ViewContext, webViewPage, _routes);
            webViewPage.Html = new HtmlHelperSimulator<object>(viewContext, webViewPage, _routes, _applicationHost);
            webViewPage.Url = new UrlHelper(webViewPage.ViewContext.RequestContext, _routes);

            try
            {
                webViewPage.ExecutePageHierarchy(
                    new WebPageContext(viewContext.HttpContext, (WebPageRenderingBase) null, (object) null), writer,
                    null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }

    public interface IApplicationHostSimulator : IControllerProvider, IContentProvider
    {
    }
}