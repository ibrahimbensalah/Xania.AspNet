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
            new TestController().Action("index").RenderView(GetIndexContentStream(), Console.Out);
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
                throw new InvalidOperationException("this action is intended to provide the context for rendering the view but should not be invoked.");
            }
        }
    }
}