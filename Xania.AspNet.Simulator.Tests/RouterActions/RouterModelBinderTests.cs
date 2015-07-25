﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests.RouterActions
{
    public class RouterModelBinderTests
    {
        private ControllerContainer _controllerContainer;

        [SetUp]
        public void SetupRouter()
        {
            _controllerContainer = new ControllerContainer()
                .RegisterController("test", () => new TestController());
        }

        [Test]
        public void ControllerNameIsRequiredTest()
        {
            Assert.Catch(() => _controllerContainer.RegisterController(null, () => new TestController()));
        }

        [Test]
        public void RequiredModelTest()
        {
            // arrange
            var action = _controllerContainer.Action("/test/index").Post().RequestData(new {name = "my name"});

            // act
            var result = action.Invoke();
            var model = (MyModel)result.ViewData.Model;

            // assert
            Assert.IsTrue(result.ModelState.IsValidField("Name"));
            Assert.IsFalse(result.ModelState.IsValidField("Email"));
            
            Assert.AreEqual("my name", model.Name);
        }

        private class TestController : Controller
        {
            public ActionResult Index(MyModel model)
            {
                return View(model);
            }
        }
        
        // ReSharper disable once ClassNeverInstantiated.Local
        private class MyModel
        {
            [Required]
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public String Name { get; set; }

            [Required]
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public String Email { get; set; }
        }
    }
}
