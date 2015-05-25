using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Xania.AspNet.Core;

namespace Xania.AspNet.Razor
{
    public class HtmlHelperSimulator<T> : HtmlHelper<T>
    {
        private readonly IMvcApplication _mvcApplication;

        internal HtmlHelperSimulator(ViewContext viewContext, IViewDataContainer viewDataContainer, IMvcApplication mvcApplication) 
            : base(viewContext, viewDataContainer, mvcApplication.Routes)
        {
            _mvcApplication = mvcApplication;
        }

        public IHtmlString Action(string actionName, object routeValues)
        {
            return _mvcApplication.Action(ViewContext, actionName, routeValues);
        }

        public IHtmlString ActionLink(string title, string actionName, string controllerName)
        {
            return LinkExtensions.ActionLink(this, title, actionName, controllerName);
        }
    }
}