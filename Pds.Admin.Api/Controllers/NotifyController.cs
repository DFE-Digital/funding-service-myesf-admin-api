using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pds.Admin.Services.Interfaces.Adapters;
using Pds.Admin.Services.Models;
using Pds.Services.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pds.Admin.Api.Controllers
{
    /// <summary>
    /// The notify controller.
    /// </summary>
    [ApiController]
    public sealed class NotifyController : ControllerBase
    {
        /// <summary>
        /// Gets the (notify details) adapter.
        /// </summary>
        internal IAdaptNotifyDetails Adapter { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyController"/> class.
        /// </summary>
        /// <param name="adapter">The notify details adapter.</param>
        public NotifyController(IAdaptNotifyDetails adapter)
        {
            It.IsNull(adapter)
                .AsGuard<ArgumentNullException>(nameof(adapter));

            Adapter = adapter;
        }

        /// <summary>
        /// Gets the notify details for...
        /// </summary>
        /// <param name="requestingService">The requesting service, e.g.VYF.</param>
        /// <param name="emailMessageType">The email message type.</param>
        /// <param name="metaData">Th metadata to filter on.</param>
        /// <returns>The result of the operation.</returns>
        [HttpGet("/api/[Controller]/{requestingService}/{emailMessageType}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetDetails(string requestingService, string emailMessageType, [FromQuery] Dictionary<string, string> metaData) =>
            await Adapter.GetNotifyDetailsFor(requestingService, emailMessageType, metaData.ToList());

        /// <summary>
        /// Sets the notify details for...
        /// </summary>
        /// <param name="notifyTemplateDetails">The details.</param>
        /// <returns>The result of the operation.</returns>
        [HttpPost("/api/[Controller]/SetDetails")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> SetDetails(NotifyTemplateDetails notifyTemplateDetails) =>
            await Adapter.SetNotifyDetails(notifyTemplateDetails);
    }
}