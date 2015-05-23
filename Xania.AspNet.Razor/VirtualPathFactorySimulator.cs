using System.Web.WebPages;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public class VirtualPathFactorySimulator : IVirtualPathFactory
    {
        private readonly IMvcApplication _mvcApplication;

        public VirtualPathFactorySimulator(IMvcApplication mvcApplication)
        {
            _mvcApplication = mvcApplication;
        }

        public bool Exists(string virtualPath)
        {
            return _mvcApplication.Exists(virtualPath);
        }

        public object CreateInstance(string virtualPath)
        {
            return _mvcApplication.Create(virtualPath);
        }
    }
}