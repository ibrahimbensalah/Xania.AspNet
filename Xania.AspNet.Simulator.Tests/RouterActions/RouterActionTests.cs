using System.Web.Mvc;
using FluentAssertions;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests.RouterActions
{
    public class RouterActionTests
    {
        private ControllerContainer _controllerContainer;

        [SetUp]
        public void SetupRouter()
        {
            _controllerContainer = new ControllerContainer()
                .RegisterController("home", () => new HomeController());
        }

        [Test]
        public void ActionFromUrlTest()
        {
            // arrange
            var action = _controllerContainer.Action("~/");
            var actionContext = action.GetExecutionContext();

            // act

            var result = action.GetActionResult(actionContext);

            // assert
            Assert.IsInstanceOf<HomeController>(actionContext.Controller);
            Assert.IsInstanceOf<ContentResult>(result);
            Assert.AreEqual("Hello Mvc Application!", actionContext.ViewBag.Message);
        }

        [Test]
        public void PostActionTest()
        {
            // arrange
            var controllerAction = _controllerContainer.Action("~/home/update").Post();
            // act
            var context = controllerAction.GetExecutionContext();
            var result = controllerAction.GetActionResult(context);
            // assert
            Assert.AreEqual("Update action is executed!", context.ViewBag.Message);
        }

        [Test]
        public void UnmatchedPostActionTest()
        {
            // arrange
            var actionContext = _controllerContainer
                .Action("~/home/update")
                .GetExecutionContext();

            // assert
            actionContext.ActionDescriptor.Should().BeNull();
        }

        class HomeController : Controller
        {
            public ActionResult Index()
            {
                ViewBag.Message = "Hello Mvc Application!";
                return Content("index");
            }

            [HttpPost]
            public void Update()
            {
                ViewBag.Message = "Update action is executed!";
            }
        }
    }
}
