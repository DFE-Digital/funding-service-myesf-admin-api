using Microsoft.Extensions.Caching.Distributed;
using Pds.Admin.Services.Interfaces.Configuration;

namespace Pds.Admin.Services.Implementations.Configuration
{
    /// <summary>
    /// VAP storage configuration.
    /// </summary>
    public sealed class CacheStorageConfiguration :
        IConfigureCacheStorage
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <inheritdoc/>
        public int ItemLifetimeInMinutes { get; set; } = 60;

        private DistributedCacheEntryOptions _options;

        /// <inheritdoc/>
        public DistributedCacheEntryOptions GetOptions() =>
            _options ??= new DistributedCacheEntryOptions
            {
                SlidingExpiration = new System.TimeSpan(0, ItemLifetimeInMinutes, 0)
            };
    }
}
