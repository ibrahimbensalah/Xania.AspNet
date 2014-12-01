using System.Web.Mvc;
using System.Web.Routing;
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

        class HomeController : Controller
        {
            public ActionResult Index()
            {
                ViewBag.Message = "Hello Mvc Application!";
                return Content("index");
            }
        }
    }
}
