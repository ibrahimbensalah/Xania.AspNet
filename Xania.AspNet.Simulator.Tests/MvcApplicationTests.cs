using System.Web.Mvc;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests
{
    public class MvcApplicationTests
    {
        private MvcApplication _app;

        [SetUp]
        public void SetupApplication()
        {
            _app = new MvcApplication()
                .RegisterDefaultRoutes()
                .RegisterControllers(typeof(MvcApplicationTests).Assembly);
        }

        [Test]
        public void ActionFromUrlTest()
        {
            // arrange
            var controllerAction = _app.Action("~/");

            // act
            var result = controllerAction.Execute();

            // assert
            Assert.IsInstanceOf<HomeController>(controllerAction.Controller);
            Assert.IsInstanceOf<ContentResult>(result.ActionResult);
            Assert.AreEqual("Hello Mvc Application!", result.ViewBag.Message);
        }

        [Test]
        public void AuthorizedActionFromUrlTest()
        {
            // arrange
            var controllerAction = _app.Action("~/home/private");

            // act
            var result = controllerAction.Authenticate("Ibrahim", null).Execute();

            // assert
            Assert.IsInstanceOf<EmptyResult>(result.ActionResult);
            Assert.AreEqual("Hello Ibrahim", result.ViewBag.Message);
        }

        [Test]
        public void UnAuthorizedActionFromUrlTest()
        {
            // arrange
            var controllerAction = _app.Action("~/home/private");

            // act
            var result = controllerAction.Execute();

            // assert
            Assert.IsInstanceOf<HttpUnauthorizedResult>(result.ActionResult);
        }

        [Test]
        public void PostActionTest()
        {
            // arrange
            var controllerAction = _app.Action("~/home/update", "POST");

            // act
            var result = controllerAction.Execute();

            // assert
            Assert.AreEqual("Update action is executed!", result.ViewBag.Message);
        }

        [Test]
        public void UnmatchedPostActionTest()
        {
            // arrange
            var controllerAction = _app.Action("~/home/update");

            // assert
            Assert.IsNull(controllerAction);
        }

        class HomeController : Controller
        {
            public ActionResult Index()
            {
                ViewBag.Message = "Hello Mvc Application!";
                return Content("index");
            }

            [Authorize]
            public void Private()
            {
                ViewBag.Message = "Hello " + User.Identity.Name;
            }

            [HttpPost]
            public void Update()
            {
                ViewBag.Message = "Update action is executed!";
            }
        }
    }
}
