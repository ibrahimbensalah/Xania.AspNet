using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests
{
    public class RouterModelBinderTests
    {
        private Router _router;

        [SetUp]
        public void SetupRouter()
        {
            _router = new Router()
                .RegisterDefaultRoutes()
                .RegisterController("test", new TestController());
        }

        [Test]
        public void RequiredModelTest()
        {
            // arrange
            var action = _router.Action("/test/index").Post().Data(new {name = "my name"});

            // act
            var result = action.Execute();
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
