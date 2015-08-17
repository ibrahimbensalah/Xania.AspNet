using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Xania.AspNet.Simulator.Tests.MvcApplication1
{
    [SetUpFixture]
    public sealed class WebDriverFixture
    {
        [ThreadStatic]
        private static IWebDriver _driver;

        [TearDown]
        public void StopDriver()
        {
            if (_driver != null)
            {
                _driver.Close();
                _driver.Dispose();
                _driver = null;
            }
        }

        public static IWebDriver Driver
        {
            get
            {
                if (_driver == null)
                {
                    _driver = new ChromeDriver();
                    _driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(1));
                }
                return _driver;
            }
        }
    }
}
