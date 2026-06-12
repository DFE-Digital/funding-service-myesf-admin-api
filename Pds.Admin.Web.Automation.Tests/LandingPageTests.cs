using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pds.Admin.Web.Automation.Tests
{
    [TestClass]
    public class LandingPageTests : BaseAutomationTests
    {
        private readonly LandingPage _page;

        public LandingPageTests()
        {
            _page = new LandingPage(Driver, Configuration["ServiceUri"]);
        }

        // Tests will be added in a future story.
    }
}