using System;
using System.IO;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public class MvcRequest<TController>: IMvcRequest
        where TController: ControllerBase
    {
        private readonly Expression<Func<TController, object>> _actionExpression;
        private IPrincipal _user;
        private readonly TController _controller;

        public MvcRequest(TController controller, Expression<Func<TController, object>> actionExpression)
        {
            _controller = controller;
            _actionExpression = actionExpression;
        }

        public void Authenticate(IPrincipal user)
        {
            if (user == null) 
                throw new ArgumentNullException("user");

            _user = user;
        }

        public MvcResult Execute()
        {
            var actionName = AspNetUtility.GetActionName(_actionExpression);
            var user = _user ?? AnonymousUser;

            var requestContext = AspNetUtility.CreateRequestContext<TController>(actionName, user, new MemoryStream());

            var controllerContext = new ControllerContext(requestContext, _controller);

            var invoker = new MvcActionInvoker(controllerContext, new LinqActionDescriptor<TController>(_actionExpression));

            return new MvcResult
            {
                ControllerContext = controllerContext,
                ActionResult = invoker.InvokeAction()
            };
        }

        static MvcRequest()
        {
            AnonymousUser = new GenericPrincipal(new GenericIdentity(String.Empty), new string[] {});
        }

        public static IPrincipal AnonymousUser { get; private set; }

    }
}
