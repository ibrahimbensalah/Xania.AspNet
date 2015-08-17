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
            action.RequestData(new {id = 2});
            // act
            var contentResult = (ContentResult)action.GetActionResult();
            // assert
            Assert.AreEqual("2", contentResult.Content);
        }

        class HomeController : Controller
        {
            public ActionResult Get(int id)
            {
                return Content(id.ToString());
            }
        }
    }
}
