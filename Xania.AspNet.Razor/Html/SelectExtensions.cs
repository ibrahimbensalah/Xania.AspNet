using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Xania.AspNet.Razor.Html
{
    public static class SelectExtensions
    {
        // DropDownList

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name)
        {
            return System.Web.Mvc.Html.SelectExtensions.DropDownList(htmlHelper, name);
        }

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, string optionLabel)
        {
            return System.Web.Mvc.Html.SelectExtensions.DropDownList(htmlHelper, name, optionLabel);
        }

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList)
        {
            return System.Web.Mvc.Html.SelectExtensions.DropDownList(htmlHelper, name, selectList);
        }

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return System.Web.Mvc.Html.SelectExtensions.DropDownList(htmlHelper, name, selectList, htmlAttributes);
        }

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.SelectExtensions.DropDownList(htmlHelper, name, selectList, htmlAttributes);
        }

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, string optionLabel)
        {
            return System.Web.Mvc.Html.SelectExtensions.DropDownList(htmlHelper, name, selectList, optionLabel);
        }

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            return System.Web.Mvc.Html.SelectExtensions.DropDownList(htmlHelper, name, selectList, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.SelectExtensions.DropDownList(htmlHelper, name, selectList, optionLabel, htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            return System.Web.Mvc.Html.SelectExtensions.DropDownListFor(htmlHelper, expression, selectList);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return System.Web.Mvc.Html.SelectExtensions.DropDownListFor(htmlHelper, expression, selectList, htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.SelectExtensions.DropDownListFor(htmlHelper, expression, selectList, htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel)
        {
            return System.Web.Mvc.Html.SelectExtensions.DropDownListFor(htmlHelper, expression, selectList, optionLabel);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            return System.Web.Mvc.Html.SelectExtensions.DropDownListFor(htmlHelper, expression, selectList, optionLabel, htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Users cannot use anonymous methods with the LambdaExpression type")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.SelectExtensions.DropDownListFor(htmlHelper, expression, selectList, optionLabel, htmlAttributes);
        }

        // ListBox

        public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name)
        {
            return System.Web.Mvc.Html.SelectExtensions.ListBox(htmlHelper, name);
        }

        public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList)
        {
            return System.Web.Mvc.Html.SelectExtensions.ListBox(htmlHelper, name, selectList);
        }

        public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return System.Web.Mvc.Html.SelectExtensions.ListBox(htmlHelper, name, selectList, htmlAttributes);
        }

        public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.SelectExtensions.ListBox(htmlHelper, name, selectList, htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString ListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            return System.Web.Mvc.Html.SelectExtensions.ListBoxFor(htmlHelper, expression, selectList);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString ListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return System.Web.Mvc.Html.SelectExtensions.ListBoxFor(htmlHelper, expression, selectList, htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Users cannot use anonymous methods with the LambdaExpression type")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString ListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.SelectExtensions.ListBoxFor(htmlHelper, expression, selectList, htmlAttributes);
        }
    }
}
