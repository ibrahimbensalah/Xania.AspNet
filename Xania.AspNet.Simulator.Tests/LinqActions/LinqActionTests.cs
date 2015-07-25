using System.Web.Mvc;
using NUnit.Framework;
using Xania.AspNet.Simulator.Tests.Controllers;

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
            Assert.Catch<ControllerActionException>(() => controllerAction.Invoke());
        }

        [Test]
        public void ActionWithCookieTest()
        {
            // arrange
            var action = new TestController()
                .Action(e => e.Index())
                .AddCookie("name1", "value1");

            // act
            var name1 = action.GetActionContext()
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
                .Action(e => e.Index())
                .AddSession("name1", "value1");

            // act
            var session = action.GetActionContext()
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
                .Action(e => e.ActionUsingUrl());

            // act
            var result = action.Invoke();

            // assert
            Assert.IsInstanceOf<ContentResult>(result.ActionResult);
            Assert.AreEqual("/Test", ((ContentResult)result.ActionResult).Content);
        }
    }
}
