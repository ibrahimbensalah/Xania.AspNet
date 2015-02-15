using System.IO;
using System.Web.Mvc;

namespace Xania.AspNet.TagHelpers
{
    public class HtmlProcessorAttribute : ActionFilterAttribute
    {
        private readonly TagHelperProvider _tagHelperProvider;

        public HtmlProcessorAttribute(TagHelperProvider tagHelperProvider)
        {
            _tagHelperProvider = tagHelperProvider;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var response = filterContext.HttpContext.Response;
            if (response.Filter != null)
            {
                response.Filter = new HtmlProcessor(new StreamWriter(response.Filter, response.ContentEncoding),
                    _tagHelperProvider);
            }
            base.OnResultExecuting(filterContext);
        }
    }
}