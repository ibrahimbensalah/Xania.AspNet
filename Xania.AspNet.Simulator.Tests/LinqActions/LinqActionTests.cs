using System;
using System.Web.Mvc;
using System.Web.Security;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests.LinqActions
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
        public void PostActionIsNotAllowedWithGetTest()
        {
            // arrange 
            var controller = new TestController();

            // act 
            var controllerAction = controller.Action(c => c.Update());

            // assert
            Assert.Catch<InvalidOperationException>(() => controllerAction.Execute());
        }

        [Test]
        public void ActionWithCookieTest()
        {
            // arrange
            var action = new TestController()
                .Action(e => e.Index())
                .AddCookie("name1", "value1");

            // act
            var result = action.Execute();

            // assert
            Assert.AreEqual("value1", result.Request.Cookies["name1"].Value);
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
