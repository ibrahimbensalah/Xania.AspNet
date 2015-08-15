using System.IO;
using System.Web.Mvc;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Xania.AspNet.Simulator.Tests.Controllers;

namespace Xania.AspNet.Simulator.Tests.NamedActions
{
    public class NamedActionTests
    {

        [Test]
        public void GetActionResultTest()
        {
            // arange
            var action = new TestController()
                .Action("index");
            var context = action.GetExecutionContext();

            // act
            action.GetActionResult(context);

            // assert
            Assert.AreEqual("Hello Simulator!", context.ViewBag.Title);
        }

        [Test]
        public void PostActionIsNotAllowedWithGetTest()
        {
            // arrange 
            var controller = new TestController();

            // act 
            var controllerAction = controller.Action("update");

            // assert
            Assert.Catch<ControllerActionException>(() => controllerAction.GetActionResult());
        }

        [Test]
        public void ActionWithCookieTest()
        {
            // arrange
            var action = new TestController()
                .Action("index")
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
            var action = new TestController()
                .Action("index")
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
            var action = new TestController()
                .Action("ActionUsingUrl");

            // act
            var result = action.GetActionResult();

            // assert
            Assert.IsInstanceOf<ContentResult>(result);
            Assert.AreEqual("/Test", ((ContentResult)result).Content);
        }
    }
}
