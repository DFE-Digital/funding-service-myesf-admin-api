using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Pds.Admin.Api.Helpers;
using Pds.Admin.Api.MvcConfiguration;
using Pds.Admin.Services.Implementations.Configuration;
using Pds.Admin.Services.Models;
using Pds.Core.ApiAuthentication;
using Pds.Core.AzureStorage;
using Pds.Core.AzureStorage.Interfaces;
using Pds.Core.AzureStorage.Models;
using Pds.Core.AzureStorage.Services;
using Pds.Core.Logging;
using Pds.Core.SecurityAssurances.Middlewares;
using Pds.Core.SecurityAssurances.Middlewares.Options;
using Pds.Core.Telemetry.ApplicationInsights;
using Pds.Services.Common.Helpers;
using Pds.Services.Common.Registration;
using System;

namespace Pds.Admin.Api
{
    /// <summary>
    /// The startup class.
    /// </summary>
    public sealed class Startup
    {
        private const string CurrentApiVersion = "v1.0.0";

        private string _assemblyName;

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        internal IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the environment.
        /// </summary>
        internal IWebHostEnvironment Environment { get; }

        /// <summary>
        /// Gets the assembly name.
        /// </summary>
        internal string AssemblyName => _assemblyName ??= GetType().Assembly.GetName().Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="environment">The hosting environment the application is running in.</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            It.IsNull(configuration)
                .AsGuard<ArgumentNullException>(nameof(configuration));
            It.IsNull(environment)
                .AsGuard<ArgumentNullException>(nameof(environment));

            Configuration = configuration;
            Environment = environment;
        }

        /// <summary>
        /// Configures the services for the container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var config = Configuration.LoadSection<CacheStorageConfiguration>("StorageCache");
            services
                .AddControllersWithParameterTransformer()
                .AddDistributedRedisCache(options => options.Configuration = config.ConnectionString)
                .AddHttpClient()
                .AddPdsApplicationInsightsTelemetry(options => ConfigureAppInsights(options));

            var notifyConfig = Configuration.LoadSection<AzureStorageConfiguration>("NotifyTableStorage");
            services
                .AddAzureStorage(
                options =>
                options.ConnectionString = notifyConfig.ConnectionString,
                options => options.TableStorageName = nameof(NotifyServiceTemplateDetails),
                null);

            services
                .AddTransient<IAzureTableStorageRepository<NotifyServiceTemplateDetails>, AzureTableStorageRepository<NotifyServiceTemplateDetails>>()
                .AddAzureADAuthentication(Configuration)
                .AddLoggerAdapter();

            if (Environment.IsDevelopment())
            {
                services
                    .DisableAuthentication(AssemblyName)
                    .AddSwaggerGen(c => c.SwaggerDoc(CurrentApiVersion, new OpenApiInfo { Title = AssemblyName, Version = CurrentApiVersion }));
            }

            RegisterServices(services);
        }

        /// <summary>
        /// Register services, configurations and faults...
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void RegisterServices(IServiceCollection services)
        {
            var registrar = ServiceRegistrar.Create(Configuration);
            registrar.RegisterWith(services).Wait();
        }

        /// <summary>
        /// Configures the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web hosting environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app
                    .UseDeveloperExceptionPage()
                    .UseSwagger()
                    .UseSwaggerUI(c => c.SwaggerEndpoint($"/swagger/{CurrentApiVersion}/swagger.json", AssemblyName));
            }

            app
                .UseMiddleware<SecurityHeadersMiddleware>(Options.Create(new SecurityHeadersOptions
                {
                    IsAPI = true
                }))
                .UseHttpsRedirection()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private void ConfigureAppInsights(PdsApplicationInsightsConfiguration options)
        {
            Configuration.Bind("PdsApplicationInsights", options);
            options.Component = AssemblyName;
        }
    }
}