using System.IO;
using System.Web.Mvc;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public class RazorViewSimulator : IView
    {
        private readonly IMvcApplication _mvcApplication;
        private readonly IVirtualContent _virtualContent;
        private readonly bool _isPartialView;

        public RazorViewSimulator(IMvcApplication mvcApplication, IVirtualContent virtualContent, bool isPartialView = false)
        {
            _mvcApplication = mvcApplication;
            _virtualContent = virtualContent;
            _isPartialView = isPartialView;
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var webPage = Create(viewContext);
            webPage.Execute(viewContext.HttpContext, writer);
        }

        private IWebViewPage Create(ViewContext viewContext)
        {
            // var virtualContent = _mvcApplication.GetVirtualContent(_virtualPath);

            var webPage = _mvcApplication.CreatePage(_virtualContent, !_isPartialView);
            webPage.Initialize(viewContext, _virtualContent.VirtualPath, _mvcApplication);

            return webPage;
        }
    }
}