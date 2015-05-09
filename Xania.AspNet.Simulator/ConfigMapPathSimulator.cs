using System;
using System.Web.Configuration;

namespace Xania.AspNet.Simulator
{
    internal class ConfigMapPathSimulator : IConfigMapPath
    {
        public string GetMachineConfigFilename()
        {
            throw new NotImplementedException();
        }

        public string GetRootWebConfigFilename()
        {
            throw new NotImplementedException();
        }

        public void GetPathConfigFilename(string siteID, string path, out string directory, out string baseName)
        {
            throw new NotImplementedException();
        }

        public void GetDefaultSiteNameAndID(out string siteName, out string siteID)
        {
            throw new NotImplementedException();
        }

        public void ResolveSiteArgument(string siteArgument, out string siteName, out string siteID)
        {
            throw new NotImplementedException();
        }

        public string MapPath(string siteID, string path)
        {
            return HttpRuntimeHelper.AppDomainAppPath + path.Replace("/", "\\"); 
            //switch (path.ToLower())
            //{
            //    //case "/web.config":
            //    //    return @"C:\Dev\Xania.AspNet-master\Xania.AspNet.Simulator.Tests\Server\Web.config";
            //    case "/dummy.cshtml":
            //        return @"C:\dev\github\Xania.AspNet\Xania.AspNet.Simulator.Tests\Server\Dummy.cshtml";
            //    default:
            //        return null;
            //}
        }

        public string GetAppPathForPath(string siteID, string path)
        {
            throw new NotImplementedException();
        }
    }
}
