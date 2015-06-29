using MvcApplication1;
using MvcApplication1.Controllers;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
                .RegisterController("home", ctx => new HomeController())
                .RegisterController("account", ctx => new AccountController(new WebSecurityImpl(ctx)));

            Server.UseMvc(controllers, contentProvider)
                .EnableRazor()
                .WithBundles(BundleConfig.RegisterBundles);
        }

        public override void StopServer()
        {
            base.StopServer();

            Driver.Manage().Cookies.DeleteAllCookies();
        }

        public IWebDriver Driver
        {
            get { return WebDriverFixture.Driver; }
        }
    }
}
