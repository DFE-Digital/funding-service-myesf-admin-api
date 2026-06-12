using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pds.Admin.Api.Client.Models;
using Pds.Admin.Services.Constants;
using Pds.Admin.Services.Interfaces.Coordinators;
using Pds.Services.Common.Helpers;
using System;
using System.Threading.Tasks;

namespace Pds.Admin.Api.Controllers
{
    /// <summary>
    /// The view as provider controller.
    /// </summary>
    [ApiController]
    public sealed class ViewAsProviderController : ControllerBase
    {
        /// <summary>
        /// Gets the (view as provider details) coordinator.
        /// </summary>
        internal ICoordinateViewAsProviderDetails Coordinator { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAsProviderController"/> class.
        /// </summary>
        /// <param name="coordinator">The view as provider details coordinator.</param>
        public ViewAsProviderController(ICoordinateViewAsProviderDetails coordinator)
        {
            It.IsNull(coordinator)
                .AsGuard<ArgumentNullException>(nameof(coordinator));

            Coordinator = coordinator;
        }

        /// <summary>
        /// Gets the view as provider ukprn for...
        /// </summary>
        /// <param name="principal">The principal id.</param>
        /// <returns>The result of the operation.</returns>
        [HttpGet("/api/[Controller]/{principal}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<int>> GetUkprn(string principal)
        {
            if (It.IsEmpty(principal))
            {
                return BadRequest();
            }

            var result = await Coordinator.GetViewAsProviderUkprnFor(principal);

            if (result.HasValue)
            {
                return Ok(result.Value);
            }

            return NoContent();
        }

        /// <summary>
        /// Gets the view as provider info for...
        /// </summary>
        /// <param name="principal">The principal id.</param>
        /// <returns>The result of the operation.</returns>
        [HttpGet("/api/[Controller]/{principal}/info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<ProviderInfo>> GetProviderInfo(string principal)
        {
            if (It.IsEmpty(principal))
            {
                return BadRequest();
            }

            var result = await Coordinator.GetViewAsProviderInfoFor(principal);

            if (!It.IsNull(result))
            {
                return Ok(result);
            }

            return NoContent();
        }

        /// <summary>
        /// Sets the view as provider ukprn for...
        /// </summary>
        /// <param name="principal">The principal id.</param>
        /// <param name="providerID">The provider id.</param>
        /// <returns>The result of the operation.</returns>
        [HttpPut("/api/[Controller]/{principal}/{providerID}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> SetUkprn(string principal, int providerID)
        {
            if (It.IsEmpty(principal) || It.IsNotBetween(providerID, ProviderIDs.MinimumProviderID, ProviderIDs.MaximumProviderID))
            {
                return BadRequest();
            }

            await Coordinator.SetViewAsProviderUkprnFor(principal, providerID);
            return Ok();
        }

        /// <summary>
        /// Sets the view as provider info for...
        /// </summary>
        /// <param name="principal">The principal id.</param>
        /// <param name="providerInfo">The request object containing provider info details.</param>
        /// <returns>The result of the operation.</returns>
        [HttpPut("/api/[Controller]/{principal}/info")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> SetProviderInfo(string principal, ProviderInfo providerInfo)
        {
            if (!It.IsNull(providerInfo))
            {
                if (It.IsEmpty(principal) || It.IsNotBetween(providerInfo.Ukprn, ProviderIDs.MinimumProviderID, ProviderIDs.MaximumProviderID) || It.IsEmpty(providerInfo.Name))
                {
                    return BadRequest();
                }

                await Coordinator.SetViewAsProviderInfoFor(principal, providerInfo);
                return Ok();
            }

            return BadRequest();
        }

        /// <summary>
        /// Clears the view as provider ukprn and info for...
        /// </summary>
        /// <param name="principal">The principal id.</param>
        /// <returns>The result of the operation.</returns>
        [HttpDelete("/api/[Controller]/{principal}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteDetails(string principal)
        {
            if (It.IsEmpty(principal))
            {
                return BadRequest();
            }

            await Coordinator.ClearViewAsProviderInfoFor(false, principal);
            await Coordinator.ClearViewAsProviderInfoFor(true, principal);

            return Ok();
        }
    }
}