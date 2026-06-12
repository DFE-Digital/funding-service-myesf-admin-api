using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pds.Core.Identity.Claims.Interfaces;
using Pds.Core.Web.Models;
using System.Threading.Tasks;

namespace Pds.Admin.Web.Controllers
{
    /// <summary>
    /// Base class for an MVC controller.
    /// </summary>
    public class BaseMvcController : Controller
    {
        private readonly IClaimsBasedIdentityService _identityService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMvcController"/> class.
        /// </summary>
        /// <param name="identityService">The <see cref="IClaimsBasedIdentityService"/>.</param>
        public BaseMvcController(IClaimsBasedIdentityService identityService)
        {
            _identityService = identityService;
        }

        /// <inheritdoc/>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionExecution = await next();
            if (actionExecution.Result is ViewResult viewResult
                && viewResult?.ViewData?.Model is BasePageViewModel model
                && model != null
                && model.CurrentUser == null)
            {
                var user = await _identityService.GetUserFromClaims(User);

                model.CurrentUser = new CurrentUserViewModel
                {
                    Ukprn = user?.Ukprn,
                    ProviderName = user?.ProviderName,
                    IsExternalUser = user?.IsExternalUser == true,
                    IsLoggedIn = user?.IsAuthenticated == true
                };
            }
        }
    }
}