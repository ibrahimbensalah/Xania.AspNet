using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Properties;
using System.Web.Routing;

namespace Xania.AspNet.Razor.Html
{
    /// <summary>
    /// Represents support for HTML links in an application.
    /// </summary>
    public static class LinkExtensions
    {
        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName)
        {
            return System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues)
        {
            return System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName, routeValues);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, object htmlAttributes)
        {
            return System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues)
        {
            return System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName, routeValues);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName)
        {
            return System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName, controllerName);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            return System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName, controllerName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName, controllerName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, object routeValues, object htmlAttributes)
        {
            return System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName, controllerName, protocol, hostName, fragment, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName, controllerName, protocol, hostName, fragment, routeValues, htmlAttributes);
        }

        public static MvcHtmlString RouteLink(this HtmlHelper htmlHelper, string linkText, object routeValues)
        {
            return System.Web.Mvc.Html.LinkExtensions.RouteLink(htmlHelper, linkText, routeValues);
        }

        public static MvcHtmlString RouteLink(this HtmlHelper htmlHelper, string linkText, RouteValueDictionary routeValues)
        {
            return System.Web.Mvc.Html.LinkExtensions.RouteLink(htmlHelper, linkText, routeValues);
        }

        public static MvcHtmlString RouteLink(this HtmlHelper htmlHelper, string linkText, string routeName)
        {
            return System.Web.Mvc.Html.LinkExtensions.RouteLink(htmlHelper, linkText, routeName);
        }

        public static MvcHtmlString RouteLink(this HtmlHelper htmlHelper, string linkText, string routeName, object routeValues)
        {
            return System.Web.Mvc.Html.LinkExtensions.RouteLink(htmlHelper, linkText, routeName, routeValues);
        }

        public static MvcHtmlString RouteLink(this HtmlHelper htmlHelper, string linkText, string routeName, RouteValueDictionary routeValues)
        {
            return System.Web.Mvc.Html.LinkExtensions.RouteLink(htmlHelper, linkText, routeName, routeValues);
        }

        public static MvcHtmlString RouteLink(this HtmlHelper htmlHelper, string linkText, object routeValues, object htmlAttributes)
        {
            return System.Web.Mvc.Html.LinkExtensions.RouteLink(htmlHelper, linkText, routeValues, htmlAttributes);
        }

        public static MvcHtmlString RouteLink(this HtmlHelper htmlHelper, string linkText, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.LinkExtensions.RouteLink(htmlHelper, linkText, routeValues, htmlAttributes);
        }

        public static MvcHtmlString RouteLink(this HtmlHelper htmlHelper, string linkText, string routeName, object routeValues, object htmlAttributes)
        {
            return System.Web.Mvc.Html.LinkExtensions.RouteLink(htmlHelper, linkText, routeName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString RouteLink(this HtmlHelper htmlHelper, string linkText, string routeName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.LinkExtensions.RouteLink(htmlHelper, linkText, routeName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString RouteLink(this HtmlHelper htmlHelper, string linkText, string routeName, string protocol, string hostName, string fragment, object routeValues, object htmlAttributes)
        {
            return System.Web.Mvc.Html.LinkExtensions.RouteLink(htmlHelper, linkText, routeName, protocol, hostName, fragment, routeValues, htmlAttributes);
        }

        public static MvcHtmlString RouteLink(this HtmlHelper htmlHelper, string linkText, string routeName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.LinkExtensions.RouteLink(htmlHelper, linkText, routeName, protocol, hostName, fragment, routeValues, htmlAttributes);
        }
    }
}
