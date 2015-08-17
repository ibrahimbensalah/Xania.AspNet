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
    public class HomeControllerTests: MvcApplication1TestBase
    {
        [TestCase("home/index")]
        [TestCase("home/about")]
        [TestCase("home/contact")]
        public void HomeControllerTest(string path)
        {
            // arrange
            Driver.Navigate().GoToUrl(GetUrl(path));

            // assert
            Driver.FindElement(By.Id("menu")).Should().NotBeNull();
        }
    }
}
