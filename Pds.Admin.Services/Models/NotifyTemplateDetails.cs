namespace Pds.Admin.Services.Models
{
    /// <summary>
    /// The notify template details.
    /// </summary>
    public class NotifyTemplateDetails
    {
        /// <summary>
        /// Gets or sets the requesting service for notify email.
        /// </summary>
        public string RequestingService { get; set; }

        /// <summary>
        /// Gets or sets the message type for notify email.
        /// </summary>
        public string EmailMessageType { get; set; }

        /// <summary>
        /// Gets or sets the row key.
        /// </summary>
        public string RowKey { get; set; }

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
    }
}