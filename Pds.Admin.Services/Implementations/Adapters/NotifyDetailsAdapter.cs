using Microsoft.AspNetCore.Mvc;
using Pds.Admin.Services.Interfaces.Adapters;
using Pds.Admin.Services.Interfaces.Coordinators;
using Pds.Admin.Services.Models;
using Pds.Services.Common.Helpers;
using Pds.Services.Common.Interfaces.Adapters;
using Pds.Services.Common.Registration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pds.Admin.Services.Implementations.Adapters
{
    /// <summary>
    /// The notify details adapter.
    /// </summary>
    public sealed class NotifyDetailsAdapter :
        IAdaptNotifyDetails,
        IRequireServiceRegistration
    {
        /// <summary>
        /// Gets the coordinator.
        /// </summary>
        internal ICoordinateNotifyDetails Coordinator { get; }

        /// <summary>
        /// Gets the (action result operations) adapter.
        /// </summary>
        internal IAdaptActionResultOperations Adapter { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyDetailsAdapter"/> class.
        /// </summary>
        /// <param name="coordinator">The notify details coordinator.</param>
        /// <param name="adapter">The action result operations adapter.</param>
        public NotifyDetailsAdapter(
            ICoordinateNotifyDetails coordinator,
            IAdaptActionResultOperations adapter)
        {
            It.IsNull(coordinator)
                .AsGuard<ArgumentNullException>(nameof(coordinator));
            It.IsNull(adapter)
                .AsGuard<ArgumentNullException>(nameof(adapter));

            Coordinator = coordinator;
            Adapter = adapter;
        }

        /// <inheritdoc/>
        public async Task<IActionResult> GetNotifyDetailsFor(string requestingService, string emailMessageType,  List<KeyValuePair<string, string>> metaData = null) =>
            await Adapter.Run(() => ProcessGetRequest(requestingService, emailMessageType, metaData), $"Getting the notify details for '{requestingService}' for type '{emailMessageType}'");

        /// <inheritdoc/>
        public async Task<IActionResult> SetNotifyDetails(NotifyTemplateDetails notifyTemplateDetails) =>
            await Adapter.Run(() => ProcessSetRequest(notifyTemplateDetails), $"Setting the notify details for '{notifyTemplateDetails.RequestingService}'");

        /// <summary>
        /// Process get request...
        /// </summary>
        /// <param name="requestingService">The requesting service.</param>
        /// <param name="emailMessageType">The email message type.</param>
        /// <param name="metaData">The metadata to filter the data on.</param>
        /// <returns>The message response and status code.</returns>
        internal async Task<HttpResponseMessage> ProcessGetRequest(string requestingService, string emailMessageType, List<KeyValuePair<string, string>> metaData) =>
            await Coordinator.GetNotifyDetailsFor(requestingService, emailMessageType, metaData);

        /// <summary>
        /// Process set request...
        /// </summary>
        /// <param name="notifyTemplateDetails">The details.</param>
        /// <returns>The message response and status code.</returns>
        internal async Task<HttpResponseMessage> ProcessSetRequest(NotifyTemplateDetails notifyTemplateDetails) =>
            await Coordinator.SetNotifyDetails(notifyTemplateDetails);
    }
}