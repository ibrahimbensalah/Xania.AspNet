using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
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
            var action = _router.Action("/test/index", "POST", data: "Name=my+name");

            // act
            var result = action.Execute();
            var model = (MyModel)result.ViewData.Model;

            // assert
            Assert.IsTrue(result.ModelState.IsValidField("Name"));
            Assert.AreEqual("my name", model.Name);
        }

        private class TestController : Controller
        {
            public ActionResult Index(MyModel model)
            {
                return View(model);
            }
        }

        private class MyModel
        {
            [Required]
            public String Name { get; set; }
        }
    }
}
