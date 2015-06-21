using FluentAssertions;
using MvcApplication1;
using MvcApplication1.Controllers;
using NUnit.Framework;
using OpenQA.Selenium;
using SimpleBrowser.WebDriver;
using Xania.AspNet.Razor;
using Xania.AspNet.Simulator.Tests.Server;

namespace Xania.AspNet.Simulator.Tests.MvcApplication1
{
    public class HomeControllerFunctionalTests: HttpServerTestBase
    {
        private static IWebDriver _driver;

        public override void StartServer()
        {
            base.StartServer();

            Server.UseStatic(TestHelper.MvcApplication1);

            var controllerContainer = new ControllerContainer()
                .RegisterControllers(typeof (HomeController).Assembly);

            Server.UseMvc(controllerContainer, TestHelper.MvcApplication1)
                .EnableRazor()
                .WithBundles(BundleConfig.RegisterBundles);
        }

        [TestFixtureSetUp]
        public static void StartWebDriver()
        {
            _driver = new SimpleBrowserDriver();
        }

        [TestFixtureTearDown]
        public static void StopWebDriver()
        {
            _driver.Dispose();
        }

        [TestCase("home/index")]
        [TestCase("home/about")]
        [TestCase("home/contact")]
        public void HomeControllerTest(string path)
        {
            // arrange
            _driver.Navigate().GoToUrl(GetUrl(path));

            // assert
            _driver.FindElement(By.Id("menu")).Should().NotBeNull();
        }
    }
}
