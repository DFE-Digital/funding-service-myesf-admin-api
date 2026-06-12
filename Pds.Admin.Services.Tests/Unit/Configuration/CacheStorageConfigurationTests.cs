using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Admin.Services.Implementations.Configuration;
using Pds.Admin.Services.Interfaces.Configuration;
using Pds.Services.Common.Registration;
using System;

namespace Pds.Admin.Services.Tests.Unit.Configuration
{
    [TestClass]
    public sealed class CacheStorageConfigurationTests :
        MoqTestingTests<CacheStorageConfiguration, IConfigureCacheStorage>
    {
        private const int DefaultExpirationInMinutes = 60;

        [TestMethod]
        public void SupportsConfigurationRegistration()
        {
            // arrange / act / assert
            TestSystemSupportsGivenContract<IRequireConfigurationRegistration>();
        }

        [TestMethod]
        public void ConnectionStringDefaultValueMeetsExpectation()
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            var result = sut.ConnectionString;

            // assert
            result.Should().BeNull();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("i'm a teapot short and stout")]
        [DataRow("or a 418 when i'm hanging about")]
        public void ConnectionStringMeetsExpectation(string expectation)
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            sut.ConnectionString = expectation;

            // assert
            sut.ConnectionString.Should().Be(expectation);
        }

        [TestMethod]
        public void ItemLifetimeInMinutesDefaultValueMeetsExpectation()
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            var result = sut.ItemLifetimeInMinutes;

            // assert
            result.Should().Be(DefaultExpirationInMinutes);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(25)]
        [DataRow(102934)]
        public void ItemLifetimeInMinutesMeetsExpectation(int expectation)
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            sut.ItemLifetimeInMinutes = expectation;

            // assert
            sut.ItemLifetimeInMinutes.Should().Be(expectation);
        }

        [TestMethod]
        public void GetOptionsDefaultValueMeetsExpectation()
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            var result = sut.GetOptions();

            // assert
            result.Should().BeAssignableTo<DistributedCacheEntryOptions>();
            result.SlidingExpiration.Should().Be(new TimeSpan(0, DefaultExpirationInMinutes, 0));
        }

        internal override void VerifyAllMocks(CacheStorageConfiguration sut)
        {
            // nothing to do...
        }

        internal override CacheStorageConfiguration BuildTestSystem() =>
            new CacheStorageConfiguration();
    }
}
