﻿using System.Web.Mvc;
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
            var controllerAction = _controllerContainer.Action("~/");

            // act
            var result = controllerAction.Invoke();

            // assert
            Assert.IsInstanceOf<HomeController>(result.Controller);
            Assert.IsInstanceOf<ContentResult>(result.ActionResult);
            Assert.AreEqual("Hello Mvc Application!", result.ViewBag.Message);
        }

        [Test]
        public void PostActionTest()
        {
            // arrange
            var controllerAction = _controllerContainer.Action("~/home/update").Post();
            // act
            var result = controllerAction.Invoke();
            // assert
            Assert.AreEqual("Update action is executed!", result.ViewBag.Message);
        }

        [Test]
        public void UnmatchedPostActionTest()
        {
            // arrange
            var controllerAction = _controllerContainer.Action("~/home/update");

            // assert
            Assert.IsNull(controllerAction.Invoke());
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
