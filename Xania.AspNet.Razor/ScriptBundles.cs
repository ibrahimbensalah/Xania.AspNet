using System.Web;
using System.Web.Mvc;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public class ScriptBundles: BundlesBase
    {
        public ScriptBundles(HttpContextBase context, IMvcApplication mvcApplication) 
            : base(context, mvcApplication)
        {
        }

        protected override string GetHtml(string path)
        {
            return "<script src=\"" + HttpUtility.UrlPathEncode(path) + "\"></script>";
        }
    }
}