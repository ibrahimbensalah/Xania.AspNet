using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.WebPages;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public abstract class WebViewPageSimulator<TModel> : WebViewPage<TModel>, IWebViewPage
    {
        public new HtmlHelperSimulator<TModel> Html { get; set; }
        public StyleBundles Styles { get; private set; }
        public ScriptBundles Scripts { get; private set; }


        public virtual void Initialize(ViewContext viewContext, string virtualPath, IMvcApplication mvcApplication)
        {
            Styles = new StyleBundles(viewContext.HttpContext, mvcApplication);
            Scripts = new ScriptBundles(viewContext.HttpContext, mvcApplication);

            VirtualPath = virtualPath;
            ViewContext = viewContext;
            ViewData = new ViewDataDictionary<TModel>(viewContext.ViewData);

            Ajax = new AjaxHelper<TModel>(viewContext, this, mvcApplication.Routes);
            Html = new HtmlHelperSimulator<TModel>(viewContext, this, mvcApplication);
            Url = new UrlHelper(viewContext.RequestContext, mvcApplication.Routes);
            VirtualPathFactory = new VirtualPathFactorySimulator(mvcApplication, viewContext);
        }

        public void Execute(HttpContextBase httpContext, TextWriter writer)
        {
            ExecutePageHierarchy(new WebPageContext(httpContext, null, null), writer);
        }
    }

    public abstract class WebViewPageSimulator : WebViewPage, IWebViewPage
    {
        public new HtmlHelperSimulator<object> Html { get; set; }

        public StyleBundles Styles { get; private set; }
        public ScriptBundles Scripts { get; private set; }

        public void Initialize(ViewContext viewContext, string virtualPath, IMvcApplication mvcApplication)
        {
            Styles = new StyleBundles(viewContext.HttpContext,  mvcApplication);
            Scripts = new ScriptBundles(viewContext.HttpContext, mvcApplication);

            VirtualPath = virtualPath;
            ViewContext = viewContext;
            ViewData = viewContext.ViewData;

            Ajax = new AjaxHelper<object>(viewContext, this, mvcApplication.Routes);
            Html = new HtmlHelperSimulator<object>(viewContext, this, mvcApplication);
            Url = new UrlHelper(viewContext.RequestContext, mvcApplication.Routes);
            VirtualPathFactory = new VirtualPathFactorySimulator(mvcApplication, viewContext);
        }

        public void Execute(HttpContextBase httpContext, TextWriter writer)
        {
            ExecutePageHierarchy(new WebPageContext(httpContext, null, null), writer);
        }
    }

    public class StyleBundles
    {
        private readonly HttpContextBase _context;
        private readonly IMvcApplication _mvcApplication;

        public StyleBundles(HttpContextBase context, IMvcApplication mvcApplication)
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
                    stringBuilder.Append("<link href=\"" + HttpUtility.UrlPathEncode(content) +
                                         "\" rel=\"stylesheet\"/>");
            }

            return MvcHtmlString.Create(stringBuilder.ToString());
        }

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
    public class ScriptBundles
    {
        private readonly HttpContextBase _context;
        private readonly IMvcApplication _mvcApplication;

        public ScriptBundles(HttpContextBase context, IMvcApplication mvcApplication)
        {
            _context = context;
            _mvcApplication = mvcApplication;
        }

        public IHtmlString Render(params string[] paths)
        {
            return MvcHtmlString.Create("");
        }

        public IHtmlString Url(string virtualPath)
        {
            return MvcHtmlString.Create("http://www.google.nl");
        }
    }
}