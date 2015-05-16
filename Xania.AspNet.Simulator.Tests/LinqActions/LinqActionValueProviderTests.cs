using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;

namespace Xania.AspNet.Simulator.Tests.LinqActions
{
    public class LinqActionValueProviderTests
    {
        [Test]
        public void PrivitiveArgumentValueTest()
        {
            // arrange
            Expression<Action<HomeController>> action = c => c.Get(1);
            var valueProvider = new LinqActionValueProvider(action.Body);
            // act
            var result = valueProvider.GetValue("id");
            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RawValue);
        }

        [Test]
        public void OverrideLinqArgumentTest()
        {
            // arrange
            var action = new HomeController().Action(c => c.Get(1));
            action.Data(new {id = 2});
            // act
            var result = action.Execute();
            // assert
            Assert.IsInstanceOf<ContentResult>(result.ActionResult);
            var contentResult = result.ActionResult as ContentResult;
            Assert.AreEqual("2", contentResult.Content);
        }

        [Test, Ignore]
        public void RequiredLinqParameterTest()
        {
            // arrange
            var action = new HomeController().Action(c => c.Upload(null));
            // act
            var result = action.Execute();
            // assert
            Assert.IsFalse(result.ModelState.IsValid);
        }

        class HomeController : Controller
        {
            public ActionResult Get(int id)
            {
                return Content(id.ToString());
            }

            public void Upload([Required]HttpPostedFileBase file)
            {
            }
        }
    }
}
