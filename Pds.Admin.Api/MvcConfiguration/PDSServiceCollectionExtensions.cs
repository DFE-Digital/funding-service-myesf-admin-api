using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Pds.Admin.Api.MvcConfiguration
{
    /// <summary>
    /// PDS Api service collection extension.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class PdsServiceCollectionExtensions
    {
        /// <summary>
        /// Adds controllers as services with route / parameter transformation.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <returns>The services collection.</returns>
        public static IServiceCollection AddControllersWithParameterTransformer(this IServiceCollection services)
        {
            services.AddControllers(options =>
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())));

            return services;
        }
    }
}