using System;
using System.Net.Http;
using System.Web.Security;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Xania.AspNet.Simulator.Tests.MvcApplication1
{
    public class AccountControllerTests: MvcApplication1TestBase
    {
        [Test]
        public void LoginTest()
        {
            // arrange
            Driver.Navigate().GoToUrl(GetUrl("account/login"));

            // assert
            Driver.FindElement(By.Id("menu")).Should().NotBeNull();
        }
    }
}
