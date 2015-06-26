using MvcApplication1;
using MvcApplication1.Controllers;
using NUnit.Framework;
using OpenQA.Selenium;
using SimpleBrowser.WebDriver;
using Xania.AspNet.Razor;
using Xania.AspNet.Simulator.Tests.Server;

namespace Xania.AspNet.Simulator.Tests.MvcApplication1
{
    public class MvcApplication1TestBase: HttpServerTestBase
    {
        public override void StartServer()
        {
            base.StartServer();

            var contentProvider = SystemUnderTest.GetMvcApp1ContentProvider();
            Server.UseStatic(contentProvider);

            var controllers = new ControllerContainer()
                .RegisterController("home", () => new HomeController())
                .RegisterController("account", () => new AccountController(new WebSecurity()));

            Server.UseMvc(controllers, contentProvider)
                .EnableRazor()
                .WithBundles(BundleConfig.RegisterBundles);
        }

        [SetUp]
        public void StartDriver()
        {
            Driver = new SimpleBrowserDriver();
        }

        [TearDown]
        public void StopDriver()
        {
            Driver.Close();
            Driver.Dispose();
        }

        public SimpleBrowserDriver Driver { get; private set; }
    }
}
