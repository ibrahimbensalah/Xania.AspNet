using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Xania.AspNet.Simulator.Razor
{
    public abstract class WebViewPageSimulator<TModel> : WebViewPage<TModel>, IWebViewPage
    {

        protected WebViewPageSimulator()
        {
        }

        public new HtmlHelperSimulator<TModel> Html { get; set; }

        public virtual void Initialize(ViewContext viewContext, string virtualPath, IMvcApplication mvcApplication)
        {
            VirtualPath = virtualPath;
            ViewContext = viewContext;
            ViewData = new ViewDataDictionary<TModel>(viewContext.ViewData);

            Ajax = new AjaxHelper<TModel>(viewContext, this, mvcApplication.Routes);
            Html = new HtmlHelperSimulator<TModel>(viewContext, this, mvcApplication);
            Url = new UrlHelper(viewContext.RequestContext, mvcApplication.Routes);
            VirtualPathFactory = new VirtualPathFactorySimulator(mvcApplication);
        }

        public void Execute(HttpContextBase httpContext, TextWriter writer)
        {
            ExecutePageHierarchy(new WebPageContext(httpContext, null, null), writer);
        }

        public override void ExecutePageHierarchy()
        {
            base.ExecutePageHierarchy();
        }
    }

    public abstract class WebViewPageSimulator : WebViewPage, IWebViewPage
    {
        public new HtmlHelperSimulator<object> Html { get; set; }

        public void Initialize(ViewContext viewContext, string virtualPath, IMvcApplication mvcApplication)
        {
            VirtualPath = virtualPath;
            ViewContext = viewContext;
            ViewData = viewContext.ViewData;

            Ajax = new AjaxHelper<object>(viewContext, this, mvcApplication.Routes);
            Html = new HtmlHelperSimulator<object>(viewContext, this, mvcApplication);
            Url = new UrlHelper(viewContext.RequestContext, mvcApplication.Routes);
            VirtualPathFactory = new VirtualPathFactorySimulator(mvcApplication);
        }

        public void Execute(HttpContextBase httpContext, TextWriter writer)
        {
            ExecutePageHierarchy(new WebPageContext(httpContext, null, null), writer);
        }
    }
}