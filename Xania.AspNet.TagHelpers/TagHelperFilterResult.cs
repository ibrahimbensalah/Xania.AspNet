using System.IO;
using System.Web.Mvc;

namespace Xania.AspNet.TagHelpers
{
    public class TagHelperFilterResult : ActionResult
    {
        private readonly ActionResult _actionResult;
        private readonly TagHelperProvider _tagHelperProvider;

        public TagHelperFilterResult(ActionResult actionResult, TagHelperProvider tagHelperProvider)
        {
            _actionResult = actionResult;
            _tagHelperProvider = tagHelperProvider;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            if (response.Filter != null)
            {
                response.Filter = new HtmlProcessor(new StreamWriter(response.Filter, response.ContentEncoding),
                    _tagHelperProvider);
            }
            else
            {
                response.Output = new TagHelperWriter(response.Output, _tagHelperProvider);
            }
            _actionResult.ExecuteResult(context);
        }
    }
}