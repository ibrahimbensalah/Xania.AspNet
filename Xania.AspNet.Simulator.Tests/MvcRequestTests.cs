using System;
using System.Web.Mvc;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests
{
    public class MvcRequestTests
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
            var controller = new TestController();

            // act 
            var result = controller.Action(c => c.UserProfile()).Authenticate("user", null).Execute();

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
    }
}
