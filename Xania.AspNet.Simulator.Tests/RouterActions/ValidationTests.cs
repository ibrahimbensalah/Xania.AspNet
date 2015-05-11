using System.Web.Mvc;
using NUnit.Framework;
using Xania.AspNet.Simulator.Tests.Annotations;

namespace Xania.AspNet.Simulator.Tests.RouterActions
{
    public class ValidationTests
    {
        private ControllerContainer _controllerContainer;

        [SetUp]
        public void SetupRouter()
        {
            _controllerContainer = new ControllerContainer()
                .RegisterController("home", new HomeController());
        }

        [Test]
        public void AuthorizedActionFromUrlTest()
        {
            // arrange
            var controllerAction = _controllerContainer.Action("~/home/private").Authenticate("Ibrahim", null);

            // act
            var result = controllerAction.Execute();

            // assert
            Assert.IsInstanceOf<EmptyResult>(result.ActionResult);
            Assert.AreEqual("Hello Ibrahim", result.ViewBag.Message);
        }

        [Test]
        public void UnAuthorizedActionFromUrlTest()
        {
            // arrange
            var controllerAction = _controllerContainer.Action("~/home/private");

            // act
            var result = controllerAction.Execute();

            // assert
            Assert.IsInstanceOf<HttpUnauthorizedResult>(result.ActionResult);
        }

        class HomeController : Controller
        {
            [Authorize, UsedImplicitly]
            public void Private()
            {
                ViewBag.Message = "Hello " + User.Identity.Name;
            }
        }
    }
}
