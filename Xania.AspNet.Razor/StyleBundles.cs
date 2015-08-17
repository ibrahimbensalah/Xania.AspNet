using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public class StyleBundles: BundlesBase
    {
        public StyleBundles(HttpContextBase context, IMvcApplication mvcApplication)
            : base(context, mvcApplication)
        {
        }

        protected override string GetHtml(string path)
        {
            return "<link href=\"" + HttpUtility.UrlPathEncode(path) +
                   "\" rel=\"stylesheet\"/>";
        }
    }
}