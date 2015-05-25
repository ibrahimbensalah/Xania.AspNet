using System;
using System.Net.Http;
using System.Threading;
using System.Web.Optimization;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using Xania.AspNet.Razor;
using Xania.AspNet.Simulator.Tests.Controllers;

namespace Xania.AspNet.Simulator.Tests.Server
{
    public class HttpServerFunctionalTests: HttpServerTestBase
    {
        private static ChromeDriver _driver;

        public override void StartServer()
        {
            base.StartServer();

            var controllerContainer = new ControllerContainer()
                .RegisterController("home", () => new HomeController());

            var contentProvider = DirectoryContentProvider.GetDefault()
                .RegisterPage(@"Views\_ViewStart.cshtml", "@{ Layout = \"~/Views/Shared/_Layout.cshtml\"; }");

            Server.UseStatic(contentProvider);

            var mvcApp = Server.UseMvc(controllerContainer, contentProvider)
                .EnableRazor();

            BundleConfig.RegisterBundles(mvcApp.Bundles);
        }

        [TestFixtureSetUp]
        public static void StartWebDriver()
        {
            _driver = new ChromeDriver();
        }

        [TestFixtureTearDown]
        public static void StopWebDriver()
        {
            _driver.Close();
            _driver.Dispose();
        }

        [TestCase("home/index")]
        [TestCase("home/about")]
        [TestCase("home/contact")]
        public void HomeIndexTest(string path)
        {
            // arrange
            _driver.Navigate().GoToUrl(BaseUrl + path);
            Console.WriteLine(_driver.PageSource);

            _driver.FindElementById("menu").Should().NotBeNull();
        }
    }
}
