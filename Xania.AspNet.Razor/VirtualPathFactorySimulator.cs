using System.Web.Mvc;
using System.Web.WebPages;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public class VirtualPathFactorySimulator : IVirtualPathFactory
    {
        private readonly IMvcApplication _mvcApplication;
        private readonly ViewContext _viewContext;

        public VirtualPathFactorySimulator(IMvcApplication mvcApplication, ViewContext viewContext)
        {
            _mvcApplication = mvcApplication;
            _viewContext = viewContext;
        }

        public bool Exists(string virtualPath)
        {
            return _mvcApplication.FileExists(virtualPath);
        }

        public object CreateInstance(string virtualPath)
        {
            IVirtualContent virtualContent = new FileVirtualContent(_mvcApplication.ContentProvider, virtualPath);

            var webPage = _mvcApplication.CreatePage(virtualContent, false);
            webPage.Initialize(_viewContext, virtualPath, _mvcApplication);

            return webPage;
        }
    }
}