using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class DirectControllerAction : ControllerAction
    {
        public DirectControllerAction(ControllerBase controller, ActionDescriptor actionDescriptor)
        {
            Controller = controller;
            ActionDescriptor = actionDescriptor;
        }

        public ControllerBase Controller { get; private set; }

        public ActionDescriptor ActionDescriptor { get; private set; }

        public override ActionContext GetActionContext(HttpContextBase httpContext = null)
        {
            return new ActionContext
            {
                ControllerContext = CreateControllerContext(httpContext ?? CreateHttpContext(), Controller, ActionDescriptor),
                ActionDescriptor = ActionDescriptor
            };
        }

        public override HttpContextBase CreateHttpContext()
        {
            return CreateHttpContext(this, ActionDescriptor);
        }
    }
}