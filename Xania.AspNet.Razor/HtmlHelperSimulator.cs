using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Mvc.Properties;
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

        /// <summary>
        /// Renders the specified partial view as an HTML-encoded string.
        /// </summary>
        /// 
        /// <returns>
        /// The partial view that is rendered as an HTML-encoded string.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="partialViewName">The name of the partial view.</param><param name="model">The model for the partial view.</param><param name="viewData">The view data dictionary for the partial view.</param>
        public MvcHtmlString Partial(string partialViewName, object model, ViewDataDictionary viewData)
        {
            using (StringWriter stringWriter = new StringWriter(CultureInfo.CurrentCulture))
            {
                RenderPartialInternal(partialViewName, viewData, model, stringWriter, _mvcApplication.ViewEngines);
                return MvcHtmlString.Create(stringWriter.ToString());
            }
        }

        public void RenderPartial(string partialViewName)
        {
            RenderPartialInternal(partialViewName, ViewData, null /* model */, ViewContext.Writer, _mvcApplication.ViewEngines);
        }

        public void RenderPartial(string partialViewName, ViewDataDictionary viewData)
        {
            RenderPartialInternal(partialViewName, viewData, null /* model */, ViewContext.Writer, _mvcApplication.ViewEngines);
        }

        public void RenderPartial(string partialViewName, object model)
        {
            RenderPartialInternal(partialViewName, ViewData, model, ViewContext.Writer, _mvcApplication.ViewEngines);
        }

        public void RenderPartial(string partialViewName, object model, ViewDataDictionary viewData)
        {
            RenderPartialInternal(partialViewName, viewData, model, ViewContext.Writer, _mvcApplication.ViewEngines);
        }


        private void RenderPartialInternal(string partialViewName, ViewDataDictionary viewData, object model, TextWriter writer, ViewEngineCollection engines)
        {
            if (string.IsNullOrEmpty(partialViewName))
                throw new ArgumentException("partialViewName");
            ViewDataDictionary viewData1;
            if (model == null)
                viewData1 = viewData != null ? new ViewDataDictionary(viewData) : new ViewDataDictionary(this.ViewData);
            else if (viewData == null)
                viewData1 = new ViewDataDictionary(model);
            else
                viewData1 = new ViewDataDictionary(viewData)
                {
                    Model = model
                };
            var viewContext = new ViewContext((ControllerContext)this.ViewContext, this.ViewContext.View, viewData1, this.ViewContext.TempData, writer);
            FindPartialView2(viewContext, partialViewName, engines).Render(viewContext, writer);
        }

        private IView FindPartialView2(ViewContext viewContext, string partialViewName, ViewEngineCollection engines)
        {
            ViewEngineResult partialView = engines.FindPartialView((ControllerContext)viewContext, partialViewName);
            if (partialView.View != null)
                return partialView.View;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str in partialView.SearchedLocations)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(str);
            }
            throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture,
                "Partial view {0} not found in {1}", partialViewName, stringBuilder));
        }


        public new MvcHtmlString AntiForgeryToken()
        {
            return MvcHtmlString.Create(string.Empty);
        }
    }
}