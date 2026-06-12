using Microsoft.AspNetCore.Mvc;
using Pds.Admin.Web.Models;
using Pds.Core.Identity.Claims.Interfaces;
using System.Threading.Tasks;

namespace Pds.Admin.Web.Controllers
{
    /// <summary>
    /// The home controller.
    /// </summary>
    public class HomeController : BaseMvcController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="identityService">The <see cref="IClaimsBasedIdentityService"/>.</param>
        public HomeController(
            IClaimsBasedIdentityService identityService)
            : base(identityService)
        {
        }

        /// <summary>
        /// The index action.
        /// </summary>
        /// <returns>The result of the index action method.</returns>
        public async Task<IActionResult> Index()
        {
            var model = await Task.FromResult(
                new ExampleViewModel
                {
                    Hello = "Hello World!!"
                });

            return View(model);
        }
    }
}