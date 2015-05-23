using System;
using System.Web;
using System.Web.WebPages;

namespace Xania.AspNet.Razor
{
    public class DisplayModeSimulator : IDisplayMode
    {
        public bool CanHandleContext(HttpContextBase httpContext)
        {
            return true;
        }

        public DisplayInfo GetDisplayInfo(HttpContextBase httpContext, string virtualPath, Func<string, bool> virtualPathExists)
        {
            return new DisplayInfo(virtualPath, this);
        }

        public string DisplayModeId
        {
            get { throw new NotImplementedException(); }
        }
    }
}