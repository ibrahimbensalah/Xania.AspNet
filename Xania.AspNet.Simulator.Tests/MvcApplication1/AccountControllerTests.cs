using System;
using System.Threading;
using FluentAssertions;
using Microsoft.Web.WebPages.OAuth;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Xania.AspNet.Simulator.Tests.MvcApplication1
{
    public class AccountControllerTests: MvcApplication1TestBase
    {
        [Test]
        public void LoginTest()
        {
            // goto login page
            Driver.Navigate().GoToUrl(GetUrl("account/login"));
            // fill form
            Driver.FindElement(By.Id("menu")).Should().NotBeNull();
            Driver.FindElement(By.Id("UserName")).SendKeys("me");
            Driver.FindElement(By.Id("Password")).SendKeys("p@ssw");
            // submit
            Driver.FindElement(By.CssSelector("[type=submit]")).Click();
            // assert user is logged in
            var userElement = Driver.FindElement(By.CssSelector("a[class=username]"));
            userElement.Text.Should().Be("me");
        }
    }
}
