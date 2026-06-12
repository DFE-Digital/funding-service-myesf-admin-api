using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Pds.Admin.Web.MvcConfiguration
{
    /// <summary>
    /// Extension methods for setting up Web MVC services in an <see cref="IServiceCollection"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class PDSServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services for API controllers to the specified <see cref="IServiceCollection"/>,
        /// using custom conventions for routing.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The incoming service collection.</returns>
        public static IServiceCollection AddControllersWithParameterTransformer(this IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())));

            return services;
        }
    }
}