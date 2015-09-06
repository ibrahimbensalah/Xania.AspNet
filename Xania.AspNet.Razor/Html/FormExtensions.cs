using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Xania.AspNet.Razor.Html
{
    /// <summary>
    /// Represents support for HTML in an application.
    /// </summary>
    public static class FormExtensions
    {
        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by an action method.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        public static MvcForm BeginForm(this HtmlHelper htmlHelper)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginForm(htmlHelper);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by an action method.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        public static MvcForm BeginForm(this HtmlHelper htmlHelper, object routeValues)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginForm(htmlHelper, routeValues);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by an action method.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="routeValues">An object that contains the parameters for a route.</param>
        public static MvcForm BeginForm(this HtmlHelper htmlHelper, RouteValueDictionary routeValues)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginForm(htmlHelper, routeValues);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by an action method.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="actionName">The name of the action method.</param><param name="controllerName">The name of the controller.</param>
        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginForm(htmlHelper, actionName, controllerName);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by an action method.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="actionName">The name of the action method.</param><param name="controllerName">The name of the controller.</param><param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginForm(htmlHelper, actionName, controllerName, routeValues);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by an action method.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="actionName">The name of the action method.</param><param name="controllerName">The name of the controller.</param><param name="routeValues">An object that contains the parameters for a route.</param>
        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginForm(htmlHelper, actionName, controllerName, routeValues);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by an action method.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="actionName">The name of the action method.</param><param name="controllerName">The name of the controller.</param><param name="method">The HTTP method for processing the form, either GET or POST.</param>
        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginForm(htmlHelper, actionName, controllerName, method);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by an action method.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="actionName">The name of the action method.</param><param name="controllerName">The name of the controller.</param><param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param><param name="method">The HTTP method for processing the form, either GET or POST.</param>
        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginForm(htmlHelper, actionName, controllerName, routeValues, method);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by an action method.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="actionName">The name of the action method.</param><param name="controllerName">The name of the controller.</param><param name="routeValues">An object that contains the parameters for a route.</param><param name="method">The HTTP method for processing the form, either GET or POST.</param>
        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginForm(htmlHelper, actionName, controllerName, routeValues, method);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by an action method.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="actionName">The name of the action method.</param><param name="controllerName">The name of the controller.</param><param name="method">The HTTP method for processing the form, either GET or POST.</param><param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, object htmlAttributes)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginForm(htmlHelper, actionName, controllerName, method, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by an action method.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="actionName">The name of the action method.</param><param name="controllerName">The name of the controller.</param><param name="method">The HTTP method for processing the form, either GET or POST.</param><param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginForm(htmlHelper, actionName, controllerName, method, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by an action method.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="actionName">The name of the action method.</param><param name="controllerName">The name of the controller.</param><param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param><param name="method">The HTTP method for processing the form, either GET or POST.</param><param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method, object htmlAttributes)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginForm(htmlHelper, actionName, controllerName, routeValues, method, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by an action method.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="actionName">The name of the action method.</param><param name="controllerName">The name of the controller.</param><param name="routeValues">An object that contains the parameters for a route.</param><param name="method">The HTTP method for processing the form, either GET or POST.</param><param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginForm(htmlHelper, actionName, controllerName, routeValues, method, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by the route target.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        public static MvcForm BeginRouteForm(this HtmlHelper htmlHelper, object routeValues)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginRouteForm(htmlHelper, routeValues);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by the route target.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="routeValues">An object that contains the parameters for a route</param>
        public static MvcForm BeginRouteForm(this HtmlHelper htmlHelper, RouteValueDictionary routeValues)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginRouteForm(htmlHelper, routeValues);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by the route target.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="routeName">The name of the route to use to obtain the form-post URL.</param>
        public static MvcForm BeginRouteForm(this HtmlHelper htmlHelper, string routeName)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginRouteForm(htmlHelper, routeName);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by the route target.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="routeName">The name of the route to use to obtain the form-post URL.</param><param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        public static MvcForm BeginRouteForm(this HtmlHelper htmlHelper, string routeName, object routeValues)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginRouteForm(htmlHelper, routeName, routeValues);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by the route target.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="routeName">The name of the route to use to obtain the form-post URL.</param><param name="routeValues">An object that contains the parameters for a route</param>
        public static MvcForm BeginRouteForm(this HtmlHelper htmlHelper, string routeName, RouteValueDictionary routeValues)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginRouteForm(htmlHelper, routeName, routeValues);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by the route target.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="routeName">The name of the route to use to obtain the form-post URL.</param><param name="method">The HTTP method for processing the form, either GET or POST.</param>
        public static MvcForm BeginRouteForm(this HtmlHelper htmlHelper, string routeName, FormMethod method)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginRouteForm(htmlHelper, routeName, method);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by the route target.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="routeName">The name of the route to use to obtain the form-post URL.</param><param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param><param name="method">The HTTP method for processing the form, either GET or POST.</param>
        public static MvcForm BeginRouteForm(this HtmlHelper htmlHelper, string routeName, object routeValues, FormMethod method)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginRouteForm(htmlHelper, routeName, routeValues, method);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by the route target.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="routeName">The name of the route to use to obtain the form-post URL.</param><param name="routeValues">An object that contains the parameters for a route</param><param name="method">The HTTP method for processing the form, either GET or POST.</param>
        public static MvcForm BeginRouteForm(this HtmlHelper htmlHelper, string routeName, RouteValueDictionary routeValues, FormMethod method)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginRouteForm(htmlHelper, routeName, routeValues, method);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by the route target.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="routeName">The name of the route to use to obtain the form-post URL.</param><param name="method">The HTTP method for processing the form, either GET or POST.</param><param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginRouteForm(this HtmlHelper htmlHelper, string routeName, FormMethod method, object htmlAttributes)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginRouteForm(htmlHelper, routeName, method, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by the route target.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="routeName">The name of the route to use to obtain the form-post URL.</param><param name="method">The HTTP method for processing the form, either GET or POST.</param><param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginRouteForm(this HtmlHelper htmlHelper, string routeName, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginRouteForm(htmlHelper, routeName, method, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by the route target.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="routeName">The name of the route to use to obtain the form-post URL.</param><param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param><param name="method">The HTTP method for processing the form, either GET or POST.</param><param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginRouteForm(this HtmlHelper htmlHelper, string routeName, object routeValues, FormMethod method, object htmlAttributes)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginRouteForm(htmlHelper, routeName, routeValues, method, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form, the request will be processed by the route target.
        /// </summary>
        /// 
        /// <returns>
        /// An opening &lt;form&gt; tag.
        /// </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param><param name="routeName">The name of the route to use to obtain the form-post URL.</param><param name="routeValues">An object that contains the parameters for a route</param><param name="method">The HTTP method for processing the form, either GET or POST.</param><param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginRouteForm(this HtmlHelper htmlHelper, string routeName, RouteValueDictionary routeValues, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            return System.Web.Mvc.Html.FormExtensions.BeginRouteForm(htmlHelper, routeName, routeValues, method, htmlAttributes);
        }

        /// <summary>
        /// Renders the closing &lt;/form&gt; tag to the response.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        public static void EndForm(this HtmlHelper htmlHelper)
        {
            System.Web.Mvc.Html.FormExtensions.EndForm(htmlHelper);
        }
    }
}
