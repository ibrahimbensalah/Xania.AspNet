using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using FluentAssertions;
using MvcApplication1.Data;
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
            // assert page is loaded correctly
            Driver.FindElement(By.Id("menu")).Should().NotBeNull();
            // fill form
            Driver.FindElement(By.Id("UserName")).SendKeys("me");
            Driver.FindElement(By.Id("Password")).SendKeys("p@ssw");
            // submit
            Driver.FindElement(By.TagName("form")).Submit();
            // assert user is logged in
            var userElement = Driver.FindElement(By.CssSelector("a[class=username]"));
            userElement.Text.Should().Be("me");
        }

        [Test]
        public void LogoutTest()
        {
            WebSecurity.Login("me", null);

            // go to index page
            Driver.Navigate().GoToUrl(GetUrl(string.Empty));
            // log off
            Driver.FindElement(By.LinkText("Log off")).Click();
            Thread.Sleep(TimeSpan.FromMilliseconds(10));
            // assert user is logged off
            Driver.FindElement(By.Id("registerLink")).Should().NotBeNull();
        }

        [Test]
        public void RegisterTest()
        {
            // go to register page
            Driver.Navigate().GoToUrl(GetUrl("account/register"));
            // fill form
            Driver.FindElement(By.Name("UserName")).SendKeys("userName1");
            Driver.FindElement(By.Name("Password")).SendKeys("password1");
            Driver.FindElement(By.Name("ConfirmPassword")).SendKeys("password1");
            // submit
            Driver.FindElement(By.TagName("form")).Submit();
            // assert user is logged in
            var userElement = Driver.FindElement(By.CssSelector("a[class=username]"));
            userElement.Text.Should().Be("userName1");
            // and the fun part, assert user is added to an in memory repository
            Users.Should().Contain(u => u.UserName.Equals("userName1"));
        }

        [Test]
        public void ManageTest()
        {
            WebSecurity.CreateUserAndAccount("me", "password");
            WebSecurity.Login("me", null);

            // go to register page
            Driver.Navigate().GoToUrl(GetUrl("account/manage"));
        }
    }
}
