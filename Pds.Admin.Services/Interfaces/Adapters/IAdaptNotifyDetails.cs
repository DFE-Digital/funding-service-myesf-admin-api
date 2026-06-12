using Microsoft.AspNetCore.Mvc;
using Pds.Admin.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pds.Admin.Services.Interfaces.Adapters
{
    /// <summary>
    /// I adapt notify details (contract).
    /// </summary>
    public interface IAdaptNotifyDetails
    {
        /// <summary>
        /// Get the notify details for...
        /// </summary>
        /// <param name="requestingService">The requesting service, e.g.VYF.</param>
        /// <param name="emailMessageType">The email message type.</param>
        /// <param name="metaData">The metadata to filter the data on.</param>
        /// <returns>The result of the action.</returns>
        Task<IActionResult> GetNotifyDetailsFor(string requestingService, string emailMessageType, List<KeyValuePair<string, string>> metaData = null);

        /// <summary>
        /// Set the notify details for..
        /// </summary>
        /// <param name="notifyTemplateDetails">The details.</param>
        /// <returns>The result of the action.</returns>
        Task<IActionResult> SetNotifyDetails(NotifyTemplateDetails notifyTemplateDetails);
    }
}