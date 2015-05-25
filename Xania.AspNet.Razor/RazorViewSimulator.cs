using System.IO;
using System.Web.Mvc;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    internal class RazorViewSimulator : IView
    {
        private readonly IMvcApplication _mvcApplication;
        private readonly string _virtualPath;

        public RazorViewSimulator(IMvcApplication mvcApplication, string virtualPath)
        {
            _mvcApplication = mvcApplication;
            _virtualPath = virtualPath;
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var webPage = Create(viewContext);
            webPage.Execute(viewContext.HttpContext, writer);
        }

        private IWebViewPage Create(ViewContext viewContext)
        {
            using (var reader = _mvcApplication.OpenText(_virtualPath, true))
            {
                return _mvcApplication.Create(viewContext, _virtualPath, reader);
            }
        }
    }
}