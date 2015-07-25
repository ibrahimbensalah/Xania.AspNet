using System.Web;
using System.Web.Mvc;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests.LinqActions
{
    public class AuthorizationTests
    {
        [Test]
        public void CustomControllerAuthorizationTest()
        {
            // arrange
            var action = new HomeController().Action(c => c.Index()).Authenticate("normaluser", null);
            // act
            var result = action.Invoke();
            // assert
            Assert.IsInstanceOf<HttpUnauthorizedResult>(result.ActionResult);
        }

        [Test]
        public void CustomFilterAuthorizationTest()
        {
            // arrange
            var action = new HomeController().Action(c => c.About()).Authenticate("normaluser", null);
            // act
            var result = action.Invoke();
            // assert
            Assert.IsInstanceOf<HttpUnauthorizedResult>(result.ActionResult);
        }

        [TestCase("ADMIN", true)]
        [TestCase("CUSTOMER", false)]
        public void AdminRoleAuthorizationTest(string roleName, bool isAuthorized)
        {
            // arrange
            var action = new AdminController().Action(c => c.Index()).Authenticate("user1", new[] {roleName});
            // assert
            Assert.AreEqual(isAuthorized, action.GetAuthorizationResult() == null);
        }


        [Test]
        public void PublicActionSkipsControllerAuthorizationTest()
        {
            // arrange 
            var controllerAction = new HomeController().Action(c => c.Public());

            // act 
            var result = controllerAction.Invoke();

            // assert
            Assert.IsInstanceOf<HttpUnauthorizedResult>(result.ActionResult);
        }


        class HomeController : Controller
        {
            public void Index(){}

            [MyAuthorize]
            public void About(){}

            [AllowAnonymous]
            public void Public() {}

            protected override void OnAuthorization(AuthorizationContext filterContext)
            {
                if (!"SUPERUSER".Equals(filterContext.HttpContext.User.Identity.Name))
                    filterContext.Result = new HttpUnauthorizedResult();
            }
        }


        [Authorize(Roles = "ADMIN")]
        class AdminController : Controller
        {
            public void Index() { }
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
