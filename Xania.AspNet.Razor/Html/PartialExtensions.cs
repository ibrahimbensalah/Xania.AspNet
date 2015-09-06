using System.Web;
using System.Web.Mvc;

namespace Xania.AspNet.Razor.Html
{
    public static class PartialExtensions
    {
        public static IHtmlString Partial<T>(this HtmlHelperSimulator<T> htmlHelper, string partialName)
        {
            return htmlHelper.Partial(partialName, null, htmlHelper.ViewData);
        }
    }
}
