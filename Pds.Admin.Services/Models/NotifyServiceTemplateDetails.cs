using Pds.Core.AzureStorage.Models;
using System;

namespace Pds.Admin.Services.Models
{
    /// <summary>
    /// Table storage to store notify template details.
    /// </summary>
    public class NotifyServiceTemplateDetails : PdsAzureTableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyServiceTemplateDetails"/> class.
        /// This table entity uses requestingService as the row key and emailMessageType as partition key.
        /// Requesting service and email message type uniquely identify rows of this table entity.
        /// </summary>
        /// <param name="emailMessageType">The message type for notify email.</param>
        public NotifyServiceTemplateDetails(string emailMessageType)
            : base(emailMessageType, Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyServiceTemplateDetails"/> class.
        /// The default constructor for deserialization.
        /// </summary>
        public NotifyServiceTemplateDetails()
            : base(string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Gets or sets the requesting service.
        /// </summary>
        public string RequestingService { get; set; }

        /// <summary>
        /// Gets or sets the metadata for notify email.
        /// </summary>
        public string Metadata { get; set; }

        /// <summary>
        /// Gets or sets the template id for notify email.
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the NotifyApiKeySecretName for notify email.
        /// </summary>
        public string NotifyApiKeySecretName { get; set; }

        /// <inheritdoc/>
        public override string TableName => nameof(NotifyServiceTemplateDetails);
    }
}