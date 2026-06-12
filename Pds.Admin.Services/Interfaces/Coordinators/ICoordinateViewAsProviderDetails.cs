using Pds.Admin.Api.Client.Models;
using System.Threading.Tasks;

namespace Pds.Admin.Services.Interfaces.Coordinators
{
    /// <summary>
    /// I coordinate view as provider details (contract).
    /// </summary>
    public interface ICoordinateViewAsProviderDetails
    {
        /// <summary>
        /// Get the view as provider info for...
        /// </summary>
        /// <param name="principal">The user information.</param>
        /// <returns>The provider ID.</returns>
        Task<int?> GetViewAsProviderUkprnFor(string principal);

        /// <summary>
        /// Get the view as provider info for...
        /// </summary>
        /// <param name="principal">The user information.</param>
        /// <returns>The ProviderInfo object.</returns>
        Task<ProviderInfo> GetViewAsProviderInfoFor(string principal);

        /// <summary>
        /// Set the view as provider info for...
        /// </summary>
        /// <param name="principal">The user information.</param>
        /// <param name="providerID">The provider id.</param>
        /// <returns>An awaitable task.</returns>
        Task SetViewAsProviderUkprnFor(string principal, int providerID);

        /// <summary>
        /// Set the view as provider info for...
        /// </summary>
        /// <param name="principal">The user information.</param>
        /// <param name="providerInfo">The provider information.</param>
        /// <returns>An awaitable task.</returns>
        Task SetViewAsProviderInfoFor(string principal, ProviderInfo providerInfo);

        /// <summary>
        /// Clear the view as provider info for...
        /// </summary>
        /// <param name="isInfo">Distinguishes between whether to create 'info' or 'ukprn' cache key prefix.</param>
        /// <param name="principal">The user information.</param>
        /// <returns>An awaitable task.</returns>
        Task ClearViewAsProviderInfoFor(bool isInfo, string principal);
    }
}
