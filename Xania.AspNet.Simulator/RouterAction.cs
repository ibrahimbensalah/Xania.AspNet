using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Xania.AspNet.Simulator
{
    public class RouterAction : ControllerAction
    {

        public RouterAction(IControllerProvider controllerContainer)
        {
            ControllerProvider = controllerContainer;
        }

        public override ControllerActionResult Execute(HttpContextBase httpContext)
        {
            var actionContext = GetActionContext(httpContext);
            var actionDescriptor = actionContext.ActionDescriptor;

            if (actionDescriptor == null)
                return null;

            return Execute(actionContext.ControllerContext, actionDescriptor);
        }

        public override ActionContext GetActionContext(HttpContextBase httpContext1)
        {
            var context = httpContext1 ?? AspNetUtility.GetContext(this, new StringWriter());
            var routeData = Routes.GetRouteData(context);

            if (routeData == null)
                return null;

            var controllerName = routeData.GetRequiredString("controller");
            var controller = ControllerProvider.CreateController(controllerName);
            var controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());

            var actionName = routeData.GetRequiredString("action");
            var httpContext = httpContext1 ?? AspNetUtility.GetContext(String.Format("/{0}/{1}", controllerName, actionName), HttpMethod, User ?? AspNetUtility.CreateAnonymousUser());

            var requestContext = new RequestContext(httpContext, routeData);
            var controllerContext = new ControllerContext(requestContext, controller);

            Initialize(controllerContext);
            return new ActionContext
            {
                ControllerContext = controllerContext,
                ActionDescriptor = controllerDescriptor.FindAction(controller.ControllerContext, actionName)
            };
        }

        public override HttpContextBase CreateHttpContext()
        {
            return AspNetUtility.GetContext(this, new StringWriter());
        }
    }

    public interface IControllerProvider
    {
        ControllerBase CreateController(string controllerName);
    }
}