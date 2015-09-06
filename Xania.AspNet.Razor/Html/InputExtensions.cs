using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Xania.AspNet.Razor.Html
{
    /// <summary>
    /// Represents support for HTML input controls in an application.
    /// </summary>
    public static class InputExtensions
    {
        public static MvcHtmlString CheckBox(this HtmlHelper htmlHelper, string name)
        {
            return System.Web.Mvc.Html.InputExtensions.CheckBox(htmlHelper, name);
        }

        public static MvcHtmlString CheckBox(this HtmlHelper htmlHelper, string name, bool isChecked)
        {
            return System.Web.Mvc.Html.InputExtensions.CheckBox(htmlHelper, name, isChecked);
        }

        public static MvcHtmlString CheckBox(this HtmlHelper htmlHelper, string name, bool isChecked, object htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.CheckBox(htmlHelper, name, isChecked, htmlAttributes);
        }

        public static MvcHtmlString CheckBox(this HtmlHelper htmlHelper, string name, object htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.CheckBox(htmlHelper, name, htmlAttributes);
        }

        public static MvcHtmlString CheckBox(this HtmlHelper htmlHelper, string name, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.CheckBox(htmlHelper, name, htmlAttributes);
        }

        public static MvcHtmlString CheckBox(this HtmlHelper htmlHelper, string name, bool isChecked, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.CheckBox(htmlHelper, name, isChecked, htmlAttributes);
        }

        public static MvcHtmlString CheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression)
        {
            return System.Web.Mvc.Html.InputExtensions.CheckBoxFor(htmlHelper, expression);
        }

        public static MvcHtmlString CheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, object htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.CheckBoxFor(htmlHelper, expression, htmlAttributes);
        }

        public static MvcHtmlString CheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.CheckBoxFor(htmlHelper, expression, htmlAttributes);
        }

        public static MvcHtmlString Hidden(this HtmlHelper htmlHelper, string name)
        {
            return System.Web.Mvc.Html.InputExtensions.Hidden(htmlHelper, name);
        }

        public static MvcHtmlString Hidden(this HtmlHelper htmlHelper, string name, object value)
        {
            return System.Web.Mvc.Html.InputExtensions.Hidden(htmlHelper, name, value);
        }

        public static MvcHtmlString Hidden(this HtmlHelper htmlHelper, string name, object value, object htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.Hidden(htmlHelper, name, value, htmlAttributes);
        }

        public static MvcHtmlString Hidden(this HtmlHelper htmlHelper, string name, object value, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.Hidden(htmlHelper, name, value, htmlAttributes);
        }

        public static MvcHtmlString HiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return System.Web.Mvc.Html.InputExtensions.HiddenFor(htmlHelper, expression);
        }

        public static MvcHtmlString HiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.HiddenFor(htmlHelper, expression, htmlAttributes);
        }

        public static MvcHtmlString HiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.HiddenFor(htmlHelper, expression, htmlAttributes);
        }

        public static MvcHtmlString Password(this HtmlHelper htmlHelper, string name)
        {
            return System.Web.Mvc.Html.InputExtensions.Password(htmlHelper, name);
        }

        public static MvcHtmlString Password(this HtmlHelper htmlHelper, string name, object value)
        {
            return System.Web.Mvc.Html.InputExtensions.Password(htmlHelper, name, value);
        }

        public static MvcHtmlString Password(this HtmlHelper htmlHelper, string name, object value, object htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.Password(htmlHelper, name, value, htmlAttributes);
        }

        public static MvcHtmlString Password(this HtmlHelper htmlHelper, string name, object value, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.Password(htmlHelper, name, value, htmlAttributes);
        }

        public static MvcHtmlString PasswordFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return System.Web.Mvc.Html.InputExtensions.PasswordFor(htmlHelper, expression);
        }

        public static MvcHtmlString PasswordFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.PasswordFor(htmlHelper, expression, htmlAttributes);
        }

        public static MvcHtmlString PasswordFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.PasswordFor(htmlHelper, expression, htmlAttributes);
        }

        public static MvcHtmlString RadioButton(this HtmlHelper htmlHelper, string name, object value)
        {
            return System.Web.Mvc.Html.InputExtensions.RadioButton(htmlHelper, name, value);
        }

        public static MvcHtmlString RadioButton(this HtmlHelper htmlHelper, string name, object value, object htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.RadioButton(htmlHelper, name, value, htmlAttributes);
        }

        public static MvcHtmlString RadioButton(this HtmlHelper htmlHelper, string name, object value, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.RadioButton(htmlHelper, name, value, htmlAttributes);
        }

        public static MvcHtmlString RadioButton(this HtmlHelper htmlHelper, string name, object value, bool isChecked)
        {
            return System.Web.Mvc.Html.InputExtensions.RadioButton(htmlHelper, name, value, isChecked);
        }

        /// <summary>
        /// Returns a radio button input element that is used to present mutually exclusive options.
        /// </summary>
        /// 
        /// <returns>
        /// An input element whose type attribute is set to "radio".
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="name">The name of the form field and the <see cref="T:System.Web.Mvc.ViewDataDictionary"/> key that is used to look up the value.</param><param name="value">If this radio button is selected, the value of the radio button that is submitted when the form is posted. If the value of the selected radio button in the <see cref="T:System.Web.Mvc.ViewDataDictionary"/> or the <see cref="T:System.Web.Mvc.ModelStateDictionary"/> object matches this value, this radio button is selected.</param><param name="isChecked">true to select the radio button; otherwise, false.</param><param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param><exception cref="T:System.ArgumentException">The <paramref name="name"/> parameter is null or empty.</exception><exception cref="T:System.ArgumentNullException">The <paramref name="value"/> parameter is null.</exception>
        public static MvcHtmlString RadioButton(this HtmlHelper htmlHelper, string name, object value, bool isChecked, object htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.RadioButton(htmlHelper, name, value, isChecked, htmlAttributes);
        }

        public static MvcHtmlString RadioButton(this HtmlHelper htmlHelper, string name, object value, bool isChecked, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.RadioButton(htmlHelper, name, value, isChecked, htmlAttributes);
        }

        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value)
        {
            return System.Web.Mvc.Html.InputExtensions.RadioButtonFor(htmlHelper, expression, value);
        }

        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value, object htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.RadioButtonFor(htmlHelper, expression, value, htmlAttributes);
        }

        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.RadioButtonFor(htmlHelper, expression, value, htmlAttributes);
        }

        public static MvcHtmlString TextBox(this HtmlHelper htmlHelper, string name)
        {
            return System.Web.Mvc.Html.InputExtensions.TextBox(htmlHelper, name);
        }

        public static MvcHtmlString TextBox(this HtmlHelper htmlHelper, string name, object value)
        {
            return System.Web.Mvc.Html.InputExtensions.TextBox(htmlHelper, name, value);
        }

        public static MvcHtmlString TextBox(this HtmlHelper htmlHelper, string name, object value, string format)
        {
            return System.Web.Mvc.Html.InputExtensions.TextBox(htmlHelper, name, value, format);
        }

        public static MvcHtmlString TextBox(this HtmlHelper htmlHelper, string name, object value, object htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.TextBox(htmlHelper, name, value, htmlAttributes);
        }

        public static MvcHtmlString TextBox(this HtmlHelper htmlHelper, string name, object value, string format, object htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.TextBox(htmlHelper, name, value, format, htmlAttributes);
        }

        public static MvcHtmlString TextBox(this HtmlHelper htmlHelper, string name, object value, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.TextBox(htmlHelper, name, value, htmlAttributes);
        }

        public static MvcHtmlString TextBox(this HtmlHelper htmlHelper, string name, object value, string format, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.TextBox(htmlHelper, name, value, format, htmlAttributes);
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return System.Web.Mvc.Html.InputExtensions.TextBoxFor(htmlHelper, expression);
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format)
        {
            return System.Web.Mvc.Html.InputExtensions.TextBoxFor(htmlHelper, expression, format);
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.TextBoxFor(htmlHelper, expression, htmlAttributes);
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format, object htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.TextBoxFor(htmlHelper, expression, format, htmlAttributes);
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.TextBoxFor(htmlHelper, expression, htmlAttributes);
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.InputExtensions.TextBoxFor(htmlHelper, expression, format, htmlAttributes);
        }
    }
}
