using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Razor;
using NSubstitute;
using NUnit.Framework;
using Xania.AspNet.Core;
using Xania.AspNet.Razor;

namespace Xania.AspNet.Simulator.Tests.ViewTests
{
    public class RazorViewTests
    {
        [Test]
        public void IndexViewTest()
        {
            var contentProvider = Substitute.For<IContentProvider>();
            contentProvider.Exists("Views\\Test\\index.cshtml").Returns(true);
            contentProvider.Open("Views\\Test\\index.cshtml").Returns(c => GetIndexContentStream());

            new MvcApplication(contentProvider)
                .EnableRazor()
                .Action(new TestController(), "index")
                .RenderView(Console.Out);
        }

        private Stream GetIndexContentStream()
        {
            var assembly = typeof(RazorViewTests).Assembly;
            var resourcePath = assembly.GetManifestResourceNames().Single(e => e.EndsWith("ViewTests.Index.cshtml"));
            return assembly.GetManifestResourceStream(resourcePath);
        }

        class TestController : Controller
        {
            public ActionResult Index()
            {
                return View();
            }
        }
    }
}