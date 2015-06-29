using System.Collections;
using System.Collections.Generic;
using MvcApplication1;
using MvcApplication1.Controllers;
using MvcApplication1.Data;
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
        protected ICollection<ApplicationUser> Users { get; private set; }

        public override void StartServer()
        {
            base.StartServer();

            Users = new List<ApplicationUser>();

            var contentProvider = SystemUnderTest.GetMvcApp1ContentProvider();
            Server.UseStatic(contentProvider);

            var controllers = new ControllerContainer()
                .RegisterController("home", ctx => new HomeController())
                .RegisterController("account", ctx => new AccountController(new WebSecurityImpl(ctx, Users)));

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
