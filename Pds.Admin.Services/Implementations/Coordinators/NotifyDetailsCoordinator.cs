using Microsoft.WindowsAzure.Storage.Table;
using Pds.Admin.Services.Interfaces.Coordinators;
using Pds.Admin.Services.Models;
using Pds.Core.AzureStorage.Extensions;
using Pds.Core.AzureStorage.Interfaces;
using Pds.Services.Common.Faults;
using Pds.Services.Common.Helpers;
using Pds.Services.Common.Interfaces.Factories;
using Pds.Services.Common.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pds.Admin.Services.Implementations.Coordinators
{
    /// <summary>
    /// The notify details coordinator.
    /// </summary>
    public sealed class NotifyDetailsCoordinator :
        ICoordinateNotifyDetails,
        IRequireServiceRegistration
    {
        /// <summary>
        /// Gets the notify table storage repository.
        /// </summary>
        internal IAzureTableStorageRepository<NotifyServiceTemplateDetails> NotifyTableStorageRepository { get; }

        /// <summary>
        /// Gets the http response message factory.
        /// </summary>
        internal ICreateHttpResponseMessages Response { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyDetailsCoordinator"/> class.
        /// </summary>
        /// <param name="notifyTableStorageRepository">The notify table storage repository.</param>
        /// <param name="response">The http response message factory.</param>
        public NotifyDetailsCoordinator(
            IAzureTableStorageRepository<NotifyServiceTemplateDetails> notifyTableStorageRepository,
            ICreateHttpResponseMessages response)
        {
            It.IsNull(notifyTableStorageRepository)
                .AsGuard<ArgumentNullException>(nameof(notifyTableStorageRepository));
            It.IsNull(response)
                .AsGuard<ArgumentNullException>(nameof(response));

            NotifyTableStorageRepository = notifyTableStorageRepository;
            Response = response;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetNotifyDetailsFor(string requestingService, string emailMessageType, List<KeyValuePair<string, string>> metaData)
        {
            It.IsEmpty(emailMessageType)
                .AsGuard<MalformedRequestException>(nameof(emailMessageType));

            It.IsEmpty(requestingService)
                .AsGuard<MalformedRequestException>(nameof(requestingService));

            var retrieveQuery = new TableQuery<NotifyServiceTemplateDetails>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, emailMessageType))
                .AndWhere(TableQuery.GenerateFilterCondition("RequestingService", QueryComparisons.Equal, requestingService));

            var tableResults = await NotifyTableStorageRepository.GetMany(retrieveQuery);

            if (metaData != null && metaData.Any())
            {
                tableResults = tableResults?.Where(notifyDetails =>
                {
                    var tableMetaData = notifyDetails.Metadata != null
                        ? JsonSerializer.Deserialize<Dictionary<string, string>>(notifyDetails.Metadata)
                        : new Dictionary<string, string>();
                    return metaData.All(x =>
                        tableMetaData.ContainsKey(x.Key)
                        && tableMetaData[x.Key].Equals(x.Value, StringComparison.InvariantCultureIgnoreCase));
                }).ToList();
            }

            var filteredResult = tableResults?.FirstOrDefault();
            if (filteredResult == null)
            {
                return Response.Create(HttpStatusCode.NoContent, "Content not found.");
            }

            var result = new NotifyTemplateDetails
            {
                RequestingService = filteredResult.RequestingService,
                EmailMessageType = filteredResult.PartitionKey,
                Metadata = filteredResult.Metadata,
                TemplateId = filteredResult.TemplateId,
                RowKey = filteredResult.RowKey,
                NotifyApiKeySecretName = filteredResult.NotifyApiKeySecretName
            };

            return await Response.Create(HttpStatusCode.OK, result);
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> SetNotifyDetails(NotifyTemplateDetails notifyTemplateDetails)
        {
            It.IsNull(notifyTemplateDetails)
                .AsGuard<MalformedRequestException>(nameof(notifyTemplateDetails));

            var tableEntity = new NotifyServiceTemplateDetails(notifyTemplateDetails.EmailMessageType)
            {
                Metadata = notifyTemplateDetails.Metadata,
                TemplateId = notifyTemplateDetails.TemplateId,
                RequestingService = notifyTemplateDetails.RequestingService,
                NotifyApiKeySecretName = notifyTemplateDetails.NotifyApiKeySecretName
            };

            await NotifyTableStorageRepository.Insert(new List<NotifyServiceTemplateDetails> { tableEntity });
            return Response.Create(HttpStatusCode.Created, string.Empty);
        }
    }
}