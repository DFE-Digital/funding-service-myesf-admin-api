using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pds.Admin.Web.MvcConfiguration;
using Pds.Core.DfESignIn;
using Pds.Core.Identity.Claims.Interfaces;
using Pds.Core.Identity.Claims.Services;
using Pds.Core.Telemetry.ApplicationInsights;
using Pds.Services.Common.Helpers;
using Pds.Services.Common.Registration;
using System;

namespace Pds.Admin.Web
{
    /// <summary>
    /// The startup class.
    /// </summary>
    public class Startup
    {
        private string _assemblyName;

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

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
            services
                .AddSingleton<IClaimsBasedIdentityService, ClaimsBasedIdentityService>()
                .AddControllersWithParameterTransformer()
                .AddDfESignInAuthentication(options => Configuration.Bind("DfESignIn", options));

            services.AddPdsApplicationInsightsTelemetry(options => BuildAppInsightsConfiguration(options));
            services.AddHealthChecks();

            if (Environment.IsDevelopment())
            {
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
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHealthChecks("/ping");
            });
        }

        private void BuildAppInsightsConfiguration(PdsApplicationInsightsConfiguration options)
        {
            Configuration.Bind("PdsApplicationInsights", options);
            options.Component = this.GetType().Assembly.GetName().Name;
        }
    }
}
