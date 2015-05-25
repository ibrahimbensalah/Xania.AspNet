using System.IO;
using System.Web.Mvc;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    internal class RazorViewSimulator : IView
    {
        private readonly IMvcApplication _mvcApplication;
        private readonly string _virtualPath;
        private readonly bool _isPartialView;

        public RazorViewSimulator(IMvcApplication mvcApplication, string virtualPath, bool isPartialView = false)
        {
            _mvcApplication = mvcApplication;
            _virtualPath = virtualPath;
            _isPartialView = isPartialView;
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var webPage = Create(viewContext);
            webPage.Execute(viewContext.HttpContext, writer);
        }

        private IWebViewPage Create(ViewContext viewContext)
        {
            using (var reader = _mvcApplication.OpenText(_virtualPath, !_isPartialView))
            {
                return _mvcApplication.Create(viewContext, _virtualPath, reader);
            }
        }
    }
}