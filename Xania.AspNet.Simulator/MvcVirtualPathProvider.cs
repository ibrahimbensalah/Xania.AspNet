using System.Web.Hosting;

namespace Xania.AspNet.Simulator
{
    public class MvcVirtualPathProvider : VirtualPathProvider
    {
        public override bool FileExists(string virtualPath)
        {
            return false;
        }
    }
}