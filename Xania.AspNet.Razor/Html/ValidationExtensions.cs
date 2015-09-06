using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Properties;
using System.Web.Routing;

namespace Xania.AspNet.Razor.Html
{
    /// <summary>
    /// Provides support for validating the input from an HTML form.
    /// </summary>
    public static class ValidationExtensions
    {
        public static void Validate(this HtmlHelper htmlHelper, string modelName)
        {
            System.Web.Mvc.Html.ValidationExtensions.Validate(htmlHelper, modelName);
        }

        public static void ValidateFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            System.Web.Mvc.Html.ValidationExtensions.ValidateFor(htmlHelper, expression);
        }

        public static MvcHtmlString ValidationMessage(this HtmlHelper htmlHelper, string modelName)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationMessage(htmlHelper, modelName);
        }

        public static MvcHtmlString ValidationMessage(this HtmlHelper htmlHelper, string modelName, object htmlAttributes)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationMessage(htmlHelper, modelName, htmlAttributes);
        }

        public static MvcHtmlString ValidationMessage(this HtmlHelper htmlHelper, string modelName, string validationMessage)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationMessage(htmlHelper, modelName, validationMessage);
        }

        public static MvcHtmlString ValidationMessage(this HtmlHelper htmlHelper, string modelName, string validationMessage, object htmlAttributes)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationMessage(htmlHelper, modelName, validationMessage, htmlAttributes);
        }

        public static MvcHtmlString ValidationMessage(this HtmlHelper htmlHelper, string modelName, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationMessage(htmlHelper, modelName, htmlAttributes);
        }

        public static MvcHtmlString ValidationMessage(this HtmlHelper htmlHelper, string modelName, string validationMessage, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationMessage(htmlHelper, modelName, validationMessage, htmlAttributes);
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression);
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string validationMessage)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression, validationMessage);
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string validationMessage, object htmlAttributes)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression, validationMessage, htmlAttributes);
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string validationMessage, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression, validationMessage, htmlAttributes);
        }

        public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationSummary(htmlHelper);
        }

        public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper, bool excludePropertyErrors)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationSummary(htmlHelper, excludePropertyErrors);
        }

        public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper, string message)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationSummary(htmlHelper, message);
        }

        public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper, bool excludePropertyErrors, string message)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationSummary(htmlHelper, message);
        }

        public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper, string message, object htmlAttributes)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationSummary(htmlHelper, message, htmlAttributes);
        }

        public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper, bool excludePropertyErrors, string message, object htmlAttributes)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationSummary(htmlHelper, excludePropertyErrors, message, htmlAttributes);
        }

        public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper, string message, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationSummary(htmlHelper, message, htmlAttributes);
        }

        public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper, bool excludePropertyErrors, string message, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.ValidationExtensions.ValidationSummary(htmlHelper, excludePropertyErrors, message,
                htmlAttributes);
        }

    }
}
