using System;
using System.Web;
using System.Web.Mvc;
using FluentAssertions;
using NUnit.Framework;
using Xania.AspNet.Simulator.Tests.Controllers;

namespace Xania.AspNet.Simulator.Tests
{
    public abstract class ControllerActionBaseTests
    {
        [Test]
        public void GetActionResultTest()
        {
            // arange
            var action = GetIndexAction(new TestController());
            var context = action.GetExecutionContext();

            // act
            action.GetActionResult(context);

            // assert
            Assert.AreEqual("Hello Simulator!", context.ViewBag.Title);
        }

        protected abstract ControllerAction GetIndexAction(TestController testController);

        protected abstract ControllerAction GetUpdateAction(TestController testController);


        [Test]
        public void PostActionIsNotAllowedWithGetTest()
        {
            // arrange 
            var controller = new TestController();

            // act 
            var controllerAction = GetUpdateAction(controller);

            // assert
            Assert.Catch<HttpException>(() => controllerAction.GetActionResult());
        }
        
        [Test]
        public void ActionWithCookieTest()
        {
            // arrange
            var action = GetIndexAction(new TestController())
                .AddCookie("name1", "value1");

            // act
            var name1 = action.GetExecutionContext()
                .ControllerContext.HttpContext.Request.Cookies["name1"];

            // assert
            Assert.IsNotNull(name1);
            Assert.AreEqual("value1", name1.Value);
        }

        [Test]
        public void ActionWithSessionTest()
        {
            // arrange
            var action = GetIndexAction(new TestController())
                .AddSession("name1", "value1");

            // act
            var session = action.GetExecutionContext()
                .ControllerContext.HttpContext.Session;

            // assert
            Assert.IsNotNull(session);
            Assert.AreEqual("value1", session["name1"]);
        }

        [Test]
        public void ActionUsingUrlTest()
        {
            // arrange
            var action = GetActionUsingUrl(new TestController());

            // act
            var result = action.GetActionResult();

            // assert
            Assert.IsInstanceOf<ContentResult>(result);
            Assert.AreEqual("/Test", ((ContentResult)result).Content);
        }

        #region Authorization tests

        [Test]
        public void CustomControllerAuthorizationTest()
        {
            // arrange
            var action = GetIndexAction(new HomeController()).Authenticate("normaluser", null);
            // act
            var result = action.GetAuthorizationResult();
            // assert
            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        protected abstract ControllerAction GetIndexAction(HomeController homeController);

        [Test]
        public void CustomFilterAuthorizationTest()
        {
            // arrange
            var action = GetAboutAction(new HomeController()).Authenticate("normaluser", null);
            // act
            var result = action.GetAuthorizationResult();
            // assert
            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        protected abstract ControllerAction GetAboutAction(HomeController homeController);

        [TestCase("ADMIN", true)]
        [TestCase("CUSTOMER", false)]
        public void AdminRoleAuthorizationTest(string roleName, bool isAuthorized)
        {
            // arrange
            var action = GetIndexAction(new AdminController()).Authenticate("user1", new[] {roleName});
            // assert
            Assert.AreEqual(isAuthorized, action.GetAuthorizationResult() == null);
        }

        protected abstract ControllerAction GetIndexAction(AdminController adminController);


        [Test]
        public void PublicActionSkipsControllerAuthorizationTest()
        {
            // arrange 
            var controllerAction = GetPublicAction(new HomeController());

            // act 
            var result = controllerAction.GetAuthorizationResult();

            // assert
            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        protected abstract ControllerAction GetPublicAction(HomeController homeController);

        #endregion

        #region ChildAction

        protected abstract ControllerAction GetChildAction(TestController testController);

        [Test]
        public void ChildActionRequestIsIdentifiedAccordinglyAtCtrlContext()
        {
            var action = GetChildAction(new TestController())
                .IsChildAction();

            var executionContext = action.GetExecutionContext();
            executionContext.ControllerContext.IsChildAction.Should().BeTrue("controller action is child action");
        }

        [Test]
        public void NonChildActionShouldNotBeAbleToFindChildActionMethod()
        {
            GetChildAction(new TestController())
                .Invoking(a => a.GetAuthorizationResult())
                .ShouldThrow<InvalidOperationException>();
        }

        #endregion

        protected abstract ControllerAction GetActionUsingUrl(TestController testController);

        protected class HomeController : Controller
        {
            public void Index() { }

            [MyAuthorize]
            public void About() { }

            [AllowAnonymous]
            public void Public() { }

            protected override void OnAuthorization(AuthorizationContext filterContext)
            {
                if (!"SUPERUSER".Equals(filterContext.HttpContext.User.Identity.Name))
                    filterContext.Result = new HttpUnauthorizedResult();
            }
        }


        protected class AdminController : Controller
        {
            [Authorize(Roles = "ADMIN")]
            public void Index() { }

            [ChildActionOnly]
            public void ChildAction() { }
        }

        protected class MyAuthorizeAttribute : AuthorizeAttribute
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                return base.AuthorizeCore(httpContext)
                       && !"SUPERUSER".Equals(httpContext.User.Identity.Name);
            }
        }
    }
}