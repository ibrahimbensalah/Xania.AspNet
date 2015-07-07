using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public abstract class WebViewPageSimulator<TModel> : WebViewPage<TModel>, IWebViewPage
    {
        private IMvcApplication _mvcApplication;
        public new HtmlHelperSimulator<TModel> Html { get; set; }
        public StyleBundles Styles { get; private set; }
        public ScriptBundles Scripts { get; private set; }


        public virtual void Initialize(ViewContext viewContext, string virtualPath, IMvcApplication mvcApplication)
        {
            _mvcApplication = mvcApplication;

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
        public override string Href(string path, params object[] pathParts)
        {
            return _mvcApplication.ToAbsoluteUrl(path);
        }
    }

    public abstract class WebViewPageSimulator : WebViewPage, IWebViewPage
    {
        private IMvcApplication _mvcApplication;
        public new HtmlHelperSimulator<object> Html { get; set; }

        public StyleBundles Styles { get; private set; }
        public ScriptBundles Scripts { get; private set; }

        public void Initialize(ViewContext viewContext, string virtualPath, IMvcApplication mvcApplication)
        {
            _mvcApplication = mvcApplication;

            Styles = new StyleBundles(viewContext.HttpContext, mvcApplication);
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

        public override string Href(string path, params object[] pathParts)
        {
            return _mvcApplication.ToAbsoluteUrl(path);
        }
    }
}