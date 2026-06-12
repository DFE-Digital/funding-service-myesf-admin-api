using OpenQA.Selenium;

namespace Pds.Admin.Web.Automation
{
    public class LandingPage
    {
        private readonly string _baseUri;
        private readonly IWebDriver _driver;

        private string PageUri
        {
            get
            {
                var baseUri = _baseUri.TrimEnd('/');
                return $"{baseUri}/documentexchange/landing";
            }
        }

        public LandingPage(IWebDriver driver, string baseUri)
        {
            _driver = driver;
            _baseUri = baseUri;
        }

        public void Navigate() =>
            _driver.Navigate().GoToUrl(PageUri);
    }
}