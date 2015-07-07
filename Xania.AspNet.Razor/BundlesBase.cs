using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public abstract class BundlesBase
    {
        private readonly HttpContextBase _context;
        private readonly IMvcApplication _mvcApplication;

        protected BundlesBase(HttpContextBase context, IMvcApplication mvcApplication)
        {
            _context = context;
            _mvcApplication = mvcApplication;
        }

        public IHtmlString Render(params string[] paths)
        {
            var stringBuilder = new StringBuilder();
            foreach (var path in paths)
            {
                foreach (var content in GetBundleContents(path))
                    stringBuilder.Append(GetHtml(content));
            }

            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        protected abstract string GetHtml(string path);

        private IEnumerable<string> GetBundleContents(string path)
        {
            return from bundle in _mvcApplication.Bundles
                where bundle.Path == path
                let context = new BundleContext(_context, _mvcApplication.Bundles, path)
                from f in bundle.EnumerateFiles(context)
                select _mvcApplication.MapUrl(f);
        }

        public IHtmlString Url(string virtualPath)
        {
            return MvcHtmlString.Create("http://www.google.nl");
        }
    }
}