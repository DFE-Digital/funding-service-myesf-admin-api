using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Admin.Services.Interfaces.Adapters;
using Pds.Admin.Services.Interfaces.Configuration;
using Pds.Admin.Services.Interfaces.Coordinators;
using Pds.Services.Common.Interfaces.Adapters;
using Pds.Services.Common.Interfaces.Converters;
using Pds.Services.Common.Interfaces.Factories;
using Pds.Services.Common.Interfaces.Providers;
using System;
using System.Linq;

namespace Pds.Admin.Api.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public sealed class StartupTests :
        MoqTestingTests<Startup>
    {
        [TestMethod]
        public void ConstructorWithNullConfigurationThrows()
        {
            // arrange
            var hostingEnvironment = MakeStrictMock<IWebHostEnvironment>();

            // act / assert
            Assert.ThrowsException<ArgumentNullException>(() => new Startup(null, hostingEnvironment));
        }

        [TestMethod]
        public void ConstructorWithNullHostingEnvironmentThrows()
        {
            // arrange
            var configuration = MakeStrictMock<IConfiguration>();

            // act / assert
            Assert.ThrowsException<ArgumentNullException>(() => new Startup(configuration, null));
        }

        [TestMethod]
        public void AssemblyNameMeetsExpection()
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            var result = sut.AssemblyName;

            // assert
            result.Should().Be("Pds.Admin.Api");
        }

        [TestMethod]
        public void RegisterServicesMeetsExpectation()
        {
            // arrange
            const int NumberOfServiceRegistrationsNotIncludingFaults = 13;
            var sut = BuildTestSystem();

            string[] pathArray =
            {
                "the storage cache item",
                "a connection string value...",
                "5", // <= the lifetime in minutes value
            };

            string[] sectionArray =
            {
                "ConnectionString",
                "ItemLifetimeInMinutes",
            };

            var section = MakeStrictMock<IConfigurationSection>();

            GetMock(sut.Configuration)
                .Setup(x => x.GetSection("StorageCache"))
                .Returns(section);

            // The basic configuration section, with path and value arrays
            GetMock(section)
                .SetupGet(x => x.Path)
                .ReturnsInOrder(pathArray);
            GetMock(section)
                .SetupGet(x => x.Value)
                .ReturnsInOrder(pathArray);
            GetMock(section)
                .Setup(x => x.GetChildren())
                .Returns(MakeEnumerableItems(1, section));

            // The identifiable sections being loaded.
            GetMock(section)
                .Setup(x => x.GetSection(It.IsIn(sectionArray)))
                .Returns(section);

            var services = MakeStrictMock<IServiceCollection>();
            GetMock(services)
                .Setup(x => x.Add(It.IsAny<ServiceDescriptor>()));

            // act
            sut.RegisterServices(services);

            // assert
            VerifyAllMocks(sut);

            GetMock(services)
                .Verify(x => x.Add(It.IsAny<ServiceDescriptor>()), Times.Exactly(NumberOfServiceRegistrationsNotIncludingFaults));
        }

        [TestMethod]
        // organisation service registrations
        [DataRow(typeof(IConfigureCacheStorage))]
        [DataRow(typeof(ICoordinateViewAsProviderDetails))]
        // common inherited registrations
        [DataRow(typeof(IAdaptActionResultOperations))]
        [DataRow(typeof(IProvideAssets))]
        [DataRow(typeof(IProvideFaultResponses))]
        [DataRow(typeof(IProvideRegistrationDetails))]
        [DataRow(typeof(IProvideSafeOperations))]
        [DataRow(typeof(ICreateHttpResponseMessages))]
        [DataRow(typeof(ICreateLoggingContexts))]
        [DataRow(typeof(IConvertJsonTypes))]
        public void RealTest_RegisterServicesMeetsExpectation(Type expectedService)
        {
            // arrange
            var sut = BuildRealSystem();
            var services = new ServiceCollection();

            // act
            sut.RegisterServices(services);

            // assert
            services.Should().Contain(x => x.ServiceType == expectedService);
        }

        [TestMethod]
        public void RealTest_IConfigureCacheStorageMeetsExpectation()
        {
            // arrange
            var sut = BuildRealSystem();
            var services = new ServiceCollection();

            // act
            sut.RegisterServices(services);

            // assert
            var result = services
                .FirstOrDefault(x => x.ServiceType == typeof(IConfigureCacheStorage))
                .ImplementationInstance as IConfigureCacheStorage;

            var options = result.GetOptions();


            // proof the configuration is being loaded from the sample config
            result.ItemLifetimeInMinutes.Should().Be(5);
            options.Should().BeAssignableTo<DistributedCacheEntryOptions>();
            options.SlidingExpiration.Should().Be(new TimeSpan(0, 5, 0));
        }

        internal override Startup BuildTestSystem()
        {
            var config = MakeStrictMock<IConfiguration>();
            var environment = MakeStrictMock<IWebHostEnvironment>();

            return new Startup(config, environment);
        }

        internal Startup BuildRealSystem() =>
            new Startup(GetConfiguration(), GetEnvironment());

        internal IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.example.json", optional: false)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        internal IWebHostEnvironment GetEnvironment()
        {
            var mockEnvironment = new Mock<IWebHostEnvironment>(MockBehavior.Strict);
            mockEnvironment
                .SetupGet(m => m.EnvironmentName)
                .Returns(Environments.Development);

            return mockEnvironment.Object;
        }

        internal override void VerifyAllMocks(Startup sut)
        {
            GetMock(sut.Configuration).VerifyAll();
            GetMock(sut.Environment).VerifyAll();
        }
    }
}
