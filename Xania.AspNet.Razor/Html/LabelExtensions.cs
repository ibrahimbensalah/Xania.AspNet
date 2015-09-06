
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Xania.AspNet.Razor.Html
{
    /// <summary>
    /// Represents support for the HTML label element in an ASP.NET MVC view.
    /// </summary>
    public static class LabelExtensions
    {
        public static MvcHtmlString Label(this HtmlHelper html, string expression)
        {
            return System.Web.Mvc.Html.LabelExtensions.Label(html, expression);
        }

        public static MvcHtmlString Label(this HtmlHelper html, string expression, string labelText)
        {
            return System.Web.Mvc.Html.LabelExtensions.Label(html, expression, labelText);
        }

        public static MvcHtmlString Label(this HtmlHelper html, string expression, object htmlAttributes)
        {
            return System.Web.Mvc.Html.LabelExtensions.Label(html, expression, htmlAttributes);
        }

        public static MvcHtmlString Label(this HtmlHelper html, string expression, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.LabelExtensions.Label(html, expression, htmlAttributes);
        }

        public static MvcHtmlString Label(this HtmlHelper html, string expression, string labelText, object htmlAttributes)
        {
            return System.Web.Mvc.Html.LabelExtensions.Label(html, expression, labelText, htmlAttributes);
        }

        public static MvcHtmlString Label(this HtmlHelper html, string expression, string labelText, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.LabelExtensions.Label(html, expression, labelText, htmlAttributes);
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return System.Web.Mvc.Html.LabelExtensions.LabelFor(html, expression);
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText)
        {
            return System.Web.Mvc.Html.LabelExtensions.LabelFor(html, expression, labelText);
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return System.Web.Mvc.Html.LabelExtensions.LabelFor(html, expression, htmlAttributes);
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.LabelExtensions.LabelFor(html, expression, htmlAttributes);
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText, object htmlAttributes)
        {
            return System.Web.Mvc.Html.LabelExtensions.LabelFor(html, expression, labelText, htmlAttributes);
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression, string labelText, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.LabelExtensions.LabelFor(html, expression, labelText, htmlAttributes);
        }

        public static MvcHtmlString LabelForModel(this HtmlHelper html)
        {
            return System.Web.Mvc.Html.LabelExtensions.LabelForModel(html);
        }

        public static MvcHtmlString LabelForModel(this HtmlHelper html, string labelText)
        {
            return System.Web.Mvc.Html.LabelExtensions.LabelForModel(html, labelText);
        }

        public static MvcHtmlString LabelForModel(this HtmlHelper html, object htmlAttributes)
        {
            return System.Web.Mvc.Html.LabelExtensions.LabelForModel(html, htmlAttributes);
        }

        public static MvcHtmlString LabelForModel(this HtmlHelper html, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.LabelExtensions.LabelForModel(html, htmlAttributes);
        }

        public static MvcHtmlString LabelForModel(this HtmlHelper html, string labelText, object htmlAttributes)
        {
            return System.Web.Mvc.Html.LabelExtensions.LabelForModel(html, labelText, htmlAttributes);
        }

        public static MvcHtmlString LabelForModel(this HtmlHelper html, string labelText, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.LabelExtensions.LabelForModel(html, labelText, htmlAttributes);
        }
    }
}
