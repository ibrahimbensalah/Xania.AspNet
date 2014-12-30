using System.Web;
using System.Web.Mvc;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests.LinqAction
{
    public class AuthorizationTests
    {
        [Test]
        public void CustomControllerAuthorizationTests()
        {
            // arrange
            var action = new HomeController().Action(c => c.Index()).Authenticate("normaluser", null);
            // act
            var result = action.Execute();
            // assert
            Assert.IsInstanceOf<HttpUnauthorizedResult>(result.ActionResult);
        }

        [Test]
        public void CustomFilterAuthorizationTests()
        {
            // arrange
            var action = new HomeController().Action(c => c.About()).Authenticate("normaluser", null);
            // act
            var result = action.Execute();
            // assert
            Assert.IsInstanceOf<HttpUnauthorizedResult>(result.ActionResult);
        }

        class HomeController : Controller
        {
            public void Index(){}

            [MyAuthorize]
            public void About(){}

            protected override void OnAuthorization(AuthorizationContext filterContext)
            {
                if (!"SUPERUSER".Equals(filterContext.HttpContext.User.Identity.Name))
                    filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        class MyAuthorizeAttribute : AuthorizeAttribute
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                return base.AuthorizeCore(httpContext)
                       && !"SUPERUSER".Equals(httpContext.User.Identity.Name);
            }
        }
    }
}
