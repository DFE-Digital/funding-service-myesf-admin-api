using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Pds.Admin.Web.Automation.Tests
{
    public abstract class BaseAutomationTests : IDisposable
    {
        protected IWebDriver Driver { get; private set; }

        protected IConfigurationRoot Configuration { get; private set; }

        public BaseAutomationTests()
        {
            var options = new ChromeOptions
            {
                AcceptInsecureCertificates = true
            };

            Driver = new ChromeDriver(options);

            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        }

        public void Dispose()
        {
            Driver.Quit();
            Driver.Dispose();
        }
    }
}