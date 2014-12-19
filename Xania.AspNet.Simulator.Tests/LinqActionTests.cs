using System;
using System.Web.Mvc;
using System.Web.Security;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests
{
    public class LinqActionTests
    {

        [Test]
        public void ActionExecuteTest()
        {
            // arange
            var controller = new TestController();

            // act
            var result = controller.Execute(c => c.Index());

            // assert
            Assert.AreEqual("Hello Simulator!", result.ViewBag.Title);
        }

        [Test]
        public void UnAuthorizedActionTest()
        {
            // arrange 
            var controller = new TestController();

            // act 
            var result = controller.Execute(c => c.UserProfile());

            // assert
            Assert.IsInstanceOf<HttpUnauthorizedResult>(result.ActionResult);
        }


        [Test]
        public void AuthorizedActionTest()
        {
            // arrange 
            var action = new TestController().Action(c => c.UserProfile(), request => request.Authenticate("user", null));

            // act 
            var result = action.Execute();

            // assert
            Assert.IsInstanceOf<ViewResult>(result.ActionResult);
        }

        [Test]
        public void PostActionIsNotAllowedWithGetTest()
        {
            // arrange 
            var controller = new TestController();

            // act 
            var controllerAction = controller.Action(c => c.Update());

            // assert
            Assert.Catch<InvalidOperationException>(() => controllerAction.Execute());
        }
    }

    public class TestController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Hello Simulator!";
            return View();
        }

        [Authorize]
        public ActionResult UserProfile()
        {
            return View();
        }

        [HttpPost]
        public void Update()
        {
        }

        [HttpPost]
        public void Login(string userName)
        {
            FormsAuthentication.SetAuthCookie(userName, true);
        }
    }
}
