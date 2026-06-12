using Pds.Admin.Services.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pds.Admin.Services.Interfaces.Coordinators
{
    /// <summary>
    /// I coordinate notify details.
    /// </summary>
    public interface ICoordinateNotifyDetails
    {
        /// <summary>
        /// Get the notify details for...
        /// </summary>
        /// <param name="requestingService">The requesting service, e.g.VYF.</param>
        /// <param name="emailMessageType">The email message type.</param>
        /// <param name="metaData">The metadata to filter the data on.</param>
        /// <returns>The result of the action.</returns>
        Task<HttpResponseMessage> GetNotifyDetailsFor(string requestingService, string emailMessageType, List<KeyValuePair<string, string>> metaData);

        /// <summary>
        /// Set the notify details for..
        /// </summary>
        /// <param name="notifyTemplateDetails">The details.</param>
        /// <returns>The result of the action.</returns>
        Task<HttpResponseMessage> SetNotifyDetails(NotifyTemplateDetails notifyTemplateDetails);
    }
}
