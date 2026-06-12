using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pds.Admin.Web.Controllers
{
    /// <summary>
    /// The account controller - keep this controller for testing authenticated user actions locally.
    /// </summary>
    public class AccountController : Controller
    {
        private IActionResult RedirectToHome
            => RedirectToAction("Index", "Home");

        /// <summary>
        /// Login action - this in practise may not be used.
        /// </summary>
        /// <returns>The login page view.</returns>
        [Authorize]
        public IActionResult Login()
        {
            return RedirectToHome;
        }

        /// <summary>
        /// A logout action - this may or may not be used (perhaps the one in Sfs.Web would be used instead).
        /// </summary>
        /// <returns>The start page view.</returns>
        public IActionResult Logout()
        {
            return SignOut("Cookies", OpenIdConnectDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// A required action to handle part of the logout flow.
        /// </summary>
        /// <returns>The start page view.</returns>
        public IActionResult PostLogout()
        {
            return RedirectToHome;
        }

        /// <summary>
        /// A required action to handle part of the logout flow.
        /// </summary>
        /// <returns>The start page view.</returns>
        public IActionResult PostLogoutRedirect()
        {
            return RedirectToHome;
        }
    }
}