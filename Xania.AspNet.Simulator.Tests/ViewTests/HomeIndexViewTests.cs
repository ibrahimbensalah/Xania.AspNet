using System;
using System.IO;
using System.Web.Mvc;
using FluentAssertions;
using MvcApplication1.Controllers;
using NUnit.Framework;
using Xania.AspNet.Core;

namespace Xania.AspNet.Simulator.Tests.ViewTests
{
    public class HomeIndexViewTests
    {
        [Test]
        public void IndexViewFromStreamTest()
        {
            // arrange
            var contents = SystemUnderTest.GetMvcApp1ContentProvider();
            using (var indexViewContent = contents.Open(@"Views\Home\Index.cshtml"))
            {
                // act
                var sw = new StringWriter();
                new TestController().Action("index").RenderView(indexViewContent, sw);

                // assert
                sw.ToString().Should().Contain("<h3>We suggest the following:</h3>");
            }
        }

        [Test]
        public void IndexViewWithLayoutTest()
        {
            // arrange
            var contents = SystemUnderTest.GetMvcApp1ContentProvider();
            var action = new MvcApplication(contents)
                .Action(new HomeController(), "index");

            using (var indexViewContent = contents.Open(@"Views\Home\Index.cshtml"))
            {
                // act
                var sw = new StringWriter();
                action.RenderView(indexViewContent, sw);
                var output = sw.ToString();

                // assert
                output.Should().Contain("<title>Home Page - My ASP.NET MVC Application</title>", "layout content should be included");
                output.Should().Contain("<h3>We suggest the following:</h3>", "index content should be included");
            }
        }

        [Test]
        public void IndexViewWithCustomViewResult()
        {
            // arrange
            var contents = SystemUnderTest.GetMvcApp1ContentProvider();

            // act
            var sw = new StringWriter();
            new MvcApplication(contents)
                .Action(new HomeController(), "index")
                .RenderView("index", null, sw);
            var output = sw.ToString();

            // assert
            output.Should().Contain("<title>Home Page - My ASP.NET MVC Application</title>", "layout content should be included");
            output.Should().Contain("<h3>We suggest the following:</h3>", "index content should be included");
        }

        [Test]
        public void ExecuteResultShouldTriggerControllerResultEvents()
        {
            var action = new TestController().Action(c => c.Index());
            var view = action.View(new StringVirtualContent("~/Views/Test/Index.cshtml", "<h1>Hello @ViewBag.dummy</h1>"));

            view.ExecuteResult().Should().Be("<h1>Hello true</h1>");
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        class TestController : Controller
        {
            public ActionResult Index()
            {
                throw new InvalidOperationException("this action is intended to provide the context for rendering the view but should not be invoked.");
            }

            protected override void OnResultExecuting(ResultExecutingContext filterContext)
            {
                ViewData.Add("dummy", "true");
            }
        }
    }
}