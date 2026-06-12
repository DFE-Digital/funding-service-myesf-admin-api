using Microsoft.Extensions.Caching.Distributed;
using Pds.Admin.Api.Client.Models;
using Pds.Admin.Services.Constants;
using Pds.Admin.Services.Interfaces.Configuration;
using Pds.Admin.Services.Interfaces.Coordinators;
using Pds.Core.Logging;
using Pds.Services.Common.Helpers;
using Pds.Services.Common.Registration;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pds.Admin.Services.Implementations.Coordinators
{
    /// <summary>
    /// View as provider details coordinator (implementation).
    /// </summary>
    public sealed class ViewAsProviderDetailsCoordinator :
        ICoordinateViewAsProviderDetails,
        IRequireServiceRegistration
    {
        /// <summary>
        /// Gets the distributed cache.
        /// </summary>
        internal IDistributedCache Cache { get; }

        /// <summary>
        /// Gets or sets the cache configuration options.
        /// </summary>
        internal IConfigureCacheStorage Config { get; set; }

        /// <summary>
        /// Gets or sets the logger adapter.
        /// </summary>
        internal ILoggerAdapter<ViewAsProviderDetailsCoordinator> Logger { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAsProviderDetailsCoordinator"/> class.
        /// </summary>
        /// <param name="cache">The distributed cache.</param>
        /// <param name="config">The distributed cache configuration options.</param>
        /// <param name="logger">The logger adapter.</param>
        public ViewAsProviderDetailsCoordinator(
            IDistributedCache cache,
            IConfigureCacheStorage config,
            ILoggerAdapter<ViewAsProviderDetailsCoordinator> logger)
        {
            It.IsNull(cache)
                .AsGuard<ArgumentNullException>(nameof(cache));
            It.IsNull(config)
                .AsGuard<ArgumentNullException>(nameof(config));
            It.IsNull(logger)
                .AsGuard<ArgumentNullException>(nameof(logger));

            Cache = cache;
            Config = config;
            Logger = logger;
        }

        /// <inheritdoc/>
        public async Task<int?> GetViewAsProviderUkprnFor(string principal)
        {
            It.IsEmpty(principal)
                .AsGuard<ArgumentNullException>();

            Logger.LogInformation($"Getting the view as provider ukprn for '{principal}'");

            var result = await Cache.GetAsync(GetCacheKey(false, principal));
            return It.IsNull(result) ? default(int?) : BitConverter.ToInt32(result);
        }

        /// <inheritdoc/>
        public async Task<ProviderInfo> GetViewAsProviderInfoFor(string principal)
        {
            It.IsEmpty(principal)
                .AsGuard<ArgumentNullException>();

            Logger.LogInformation($"Getting the view as provider info for '{principal}'");

            var result = await Cache.GetAsync(GetCacheKey(true, principal));
            return It.IsNull(result) ? null : JsonSerializer.Deserialize<ProviderInfo>(Encoding.UTF8.GetString(result));
        }

        /// <inheritdoc/>
        public Task SetViewAsProviderUkprnFor(string principal, int providerID)
        {
            It.IsEmpty(principal)
                .AsGuard<ArgumentNullException>();

            It.IsNotBetween(providerID, ProviderIDs.MinimumProviderID, ProviderIDs.MaximumProviderID)
                .AsGuard<ArgumentException>();

            Logger.LogInformation($"Setting the view as provider ukprn for '{principal}'");

            var bytes = BitConverter.GetBytes(providerID);
            return Cache.SetAsync(GetCacheKey(false, principal), bytes, Config.GetOptions());
        }

        /// <inheritdoc/>
        public Task SetViewAsProviderInfoFor(string principal, ProviderInfo providerInfo)
        {
            It.IsEmpty(principal)
                .AsGuard<ArgumentNullException>();

            It.IsNull(providerInfo)
                .AsGuard<ArgumentNullException>();

            It.IsNotBetween(providerInfo.Ukprn, ProviderIDs.MinimumProviderID, ProviderIDs.MaximumProviderID)
                .AsGuard<ArgumentException>();

            It.IsEmpty(providerInfo.Name)
                .AsGuard<ArgumentException>();

            Logger.LogInformation($"Setting the view as provider info for '{principal}'");

            var bytes = JsonSerializer.SerializeToUtf8Bytes(providerInfo);
            return Cache.SetAsync(GetCacheKey(true, principal), bytes, Config.GetOptions());
        }

        /// <inheritdoc/>
        public Task ClearViewAsProviderInfoFor(bool isInfo, string principal)
        {
            It.IsEmpty(principal)
                .AsGuard<ArgumentNullException>();

            Logger.LogInformation($"Clearing the view as provider details for '{principal}'");

            return Cache.RemoveAsync(GetCacheKey(isInfo, principal));
        }

        private string GetCacheKey(bool isInfo, string principal)
        {
            string prefix = isInfo ? "info" : "ukprn";
            return $"{prefix}:{principal}";
        }
    }
}