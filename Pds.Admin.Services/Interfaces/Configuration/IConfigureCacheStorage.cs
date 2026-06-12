using Microsoft.Extensions.Caching.Distributed;
using Pds.Services.Common.Registration;

namespace Pds.Admin.Services.Interfaces.Configuration
{
    /// <summary>
    /// I configure distributed cache options (contract).
    /// </summary>
    public interface IConfigureCacheStorage :
        IRequireConfigurationRegistration
    {
        /// <summary>
        /// Gets item lifetime in minutes.
        /// </summary>
        int ItemLifetimeInMinutes { get; }

        /// <summary>
        /// Get the (distributed cache entry) options.
        /// </summary>
        /// <returns>The distributed cache entry options.</returns>
        DistributedCacheEntryOptions GetOptions();
    }
}
