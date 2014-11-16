using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace Xania.AspNet.Simulator
{
    public interface IMvcRequest
    {
        ActionResult Execute();
    }

    public class LinqRequest<TController> : IMvcRequest
        where TController : ControllerBase 
	{
		private readonly ControllerContext _controllerContext;
        private readonly Expression<Func<TController, object>> _actionExpression;

        public LinqRequest(ControllerContext controllerContext, Expression<Func<TController, object>>  actionExpression)
        {
            _controllerContext = controllerContext;
            _actionExpression = actionExpression;
        }

        public HttpSessionStateBase Session { get { return _controllerContext.HttpContext.Session; } }

        public Cache Cache { get { return _controllerContext.HttpContext.Cache; } }

		public virtual ActionResult Execute()
		{
            var invoker = new LinqActionInvoker<TController>(_controllerContext, _actionExpression);
            return invoker.InvokeAction();
		}
	}
}
