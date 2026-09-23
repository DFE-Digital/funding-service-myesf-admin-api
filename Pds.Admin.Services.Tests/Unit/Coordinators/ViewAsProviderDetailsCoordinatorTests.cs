using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Admin.Api.Client.Models;
using Pds.Admin.Services.Implementations.Coordinators;
using Pds.Admin.Services.Interfaces.Configuration;
using Pds.Admin.Services.Interfaces.Coordinators;
using Pds.Core.Logging;
using Pds.Services.Common.Registration;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Pds.Admin.Services.Tests.Unit.Coordinators
{
    [TestClass]
    public sealed class ViewAsProviderDetailsCoordinatorTests :
        MoqTestingTests<ViewAsProviderDetailsCoordinator, ICoordinateViewAsProviderDetails>
    {
        [TestMethod]
        public void SupportsServiceRegistration()
        {
            // arrange / act / assert
            TestSystemSupportsGivenContract<IRequireServiceRegistration>();
        }

        #region Constructor

        [TestMethod]
        public void Constructor_WhenCacheIsNull_Throws()
        {
            // arrange
            var config = MakeStrictMock<IConfigureCacheStorage>();
            var logger = MakeStrictMock<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ViewAsProviderDetailsCoordinator(null, config, logger));
        }

        [TestMethod]
        public void Constructor_WhenConfigIsNull_Throws()
        {
            // arrange
            var cache = MakeStrictMock<IDistributedCache>();
            var logger = MakeStrictMock<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ViewAsProviderDetailsCoordinator(cache, null, logger));
        }

        [TestMethod]
        public void Constructor_WhenLoggerIsNull_Throws()
        {
            // arrange
            var cache = MakeStrictMock<IDistributedCache>();
            var config = MakeStrictMock<IConfigureCacheStorage>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ViewAsProviderDetailsCoordinator(cache, config, null));
        }

        #endregion


        #region GetViewAsProviderUkprn

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void GetViewAsProviderUkprn_WhenPrincipalIsNullOrEmpty_Throws(string principal)
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            Func<Task<int?>> func = () => sut.GetViewAsProviderUkprnFor(principal);

            // assert
            func.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow("non existing principal 1")]
        [DataRow("non existing principal 2")]
        [DataRow("non existing principal 3")]
        public async Task GetViewAsProviderUkprn_WhenPrincipalDoesntExists_ReturnsNull(string principal)
        {
            // arrange
            var sut = BuildTestSystem();

            GetMock(sut.Logger)
               .Setup(x => x.LogInformation($"Getting the view as provider ukprn for '{principal}'"));

            GetMock(sut.Cache)
                .Setup(x => x.GetAsync($"ukprn:{principal}", CancellationToken.None))
                .Returns(Task.FromResult(null as byte[]));

            // act
            var result = await sut.GetViewAsProviderUkprnFor(principal);

            // assert
            VerifyAllMocks(sut);
            result.Should().BeNull();
        }

        [TestMethod]
        [DataRow("principal1", 45)]
        [DataRow("principal2", 102932)]
        [DataRow("principal3", 4502934)]
        public async Task GetViewAsProviderUkprn_WhenPrincipalExists_ReturnsProviderID(string principal, int providerID)
        {
            // arrange
            var sut = BuildTestSystem();
            var cacheItem = BitConverter.GetBytes(providerID);

            GetMock(sut.Logger)
               .Setup(x => x.LogInformation($"Getting the view as provider ukprn for '{principal}'"));

            GetMock(sut.Cache)
                .Setup(x => x.GetAsync($"ukprn:{principal}", CancellationToken.None))
                .Returns(Task.FromResult(cacheItem));

            // act
            var result = await sut.GetViewAsProviderUkprnFor(principal);

            // assert
            VerifyAllMocks(sut);
            result.Should().Be(providerID);
        }

        #endregion


        #region GetViewAsProviderInfoFor

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void GetViewAsProviderInfoFor_WhenPrincipalIsNullOrEmpty_Throws(string principal)
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            Func<Task<ProviderInfo>> func = () => sut.GetViewAsProviderInfoFor(principal);

            // assert
            func.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow("noprincipal1")]
        [DataRow("noprincipal2")]
        [DataRow("noprincipal3")]
        public async Task GetViewAsProviderInfoFor_WhenPrincipalDoesntExists_ReturnsNull(string principal)
        {
            // arrange
            var sut = BuildTestSystem();

            GetMock(sut.Logger)
               .Setup(x => x.LogInformation($"Getting the view as provider info for '{principal}'"));

            GetMock(sut.Cache)
                .Setup(x => x.GetAsync($"info:{principal}", CancellationToken.None))
                .Returns(Task.FromResult(null as byte[]));

            // act
            var result = await sut.GetViewAsProviderInfoFor(principal);

            // assert
            VerifyAllMocks(sut);
            result.Should().BeNull();
        }

        [TestMethod]
        [DataRow("principal1", 123, "abc")]
        [DataRow("principal2", 456, "def")]
        [DataRow("principal3", 789, "ghi")]
        public async Task GetViewAsProviderInfoFor_WhenPrincipalExists_ReturnsProviderInfo(string principal, int ukprn, string providerName)
        {
            // arrange
            var sut = BuildTestSystem();
            var providerInfo = new ProviderInfo
            {
                Ukprn = ukprn,
                Name = providerName
            };
            var cacheItem = JsonSerializer.SerializeToUtf8Bytes(providerInfo);

            GetMock(sut.Logger)
               .Setup(x => x.LogInformation($"Getting the view as provider info for '{principal}'"));

            GetMock(sut.Cache)
                .Setup(x => x.GetAsync($"info:{principal}", CancellationToken.None))
                .Returns(Task.FromResult(cacheItem));

            // act
            var result = await sut.GetViewAsProviderInfoFor(principal);

            // assert
            VerifyAllMocks(sut);
            result.Should().BeEquivalentTo(providerInfo);
        }

        #endregion


        #region SetViewAsProviderUkprnFor

        [TestMethod]
        [DataRow(null, 50000000)]
        [DataRow("", 60000000)]
        [DataRow(" ", 70000000)]
        public void SetViewAsProviderUkprnFor_WhenPrincipalIsNullOrEmpty_Throws(string principal, int providerID)
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            Func<Task> func = () => sut.SetViewAsProviderUkprnFor(principal, providerID);

            // assert
            func.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow("principal 1", 0)]
        [DataRow("principal 2", 9999999)]
        [DataRow("principal 3", 100000000)]
        public void SetViewAsProviderUkprnFor_WhenProviderIdIsNotBetweenMinAndMax_Throws(string principal, int providerID)
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            Func<Task> func = () => sut.SetViewAsProviderUkprnFor(principal, providerID);

            // assert
            func.Should().ThrowAsync<ArgumentException>();
        }

        [TestMethod]
        [DataRow("principal 1", 10004126)]
        [DataRow("principal 2", 10003009)]
        [DataRow("principal 3", 12345678)]
        public void SetViewAsProviderUkprnFor_WhenInputIsValid_Returns(string principal, int providerID)
        {
            // arrange
            var sut = BuildTestSystem();

            var cacheItem = BitConverter.GetBytes(providerID);
            var responseMessage = new HttpResponseMessage();
            var options = new DistributedCacheEntryOptions();

            var setCacheTask = Task.CompletedTask;

            GetMock(sut.Logger)
               .Setup(x => x.LogInformation($"Setting the view as provider ukprn for '{principal}'"));

            GetMock(sut.Config)
                .Setup(x => x.GetOptions())
                .Returns(options);

            GetMock(sut.Cache)
                .Setup(x => x.SetAsync($"ukprn:{principal}", cacheItem, options, CancellationToken.None))
                .Returns(setCacheTask);

            // act
            var result = sut.SetViewAsProviderUkprnFor(principal, providerID);

            // assert
            VerifyAllMocks(sut);
            result.Should().Be(setCacheTask);
        }

        #endregion


        #region SetViewAsProviderInfoFor

        [TestMethod]
        [DataRow(null, 12345678, "abc")]
        [DataRow("", 22345678, "def")]
        [DataRow(" ", 32345678, "ghi")]
        public void SetViewAsProviderInfoFor_WhenPrincipalIsNullOrEmpty_Throws(string principal, int ukprn, string providerName)
        {
            // arrange
            var sut = BuildTestSystem();
            var providerInfo = new ProviderInfo
            {
                Ukprn = ukprn,
                Name = providerName,
            };

            // act
            Func<Task> func = () => sut.SetViewAsProviderInfoFor(principal, providerInfo);

            // assert
            func.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public void SetViewAsProviderInfoFor_WhenProviderInfoIsNull_Throws()
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            Func<Task> func = () => sut.SetViewAsProviderInfoFor("principal1", null);

            // assert
            func.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow("principal1", 0, "abc")]
        [DataRow("principal2", 9999999, "def")]
        [DataRow("principal3", 100000000, "ghi")]
        [DataRow("principal4", 12345678, null)]
        [DataRow("principal5", 22345678, "")]
        [DataRow("principal6", 32345678, " ")]
        public void SetViewAsProviderInfoFor_WhenProviderInfoIsNotValid_Throws(string principal, int ukprn, string providerName)
        {
            // arrange
            var sut = BuildTestSystem();
            var providerInfo = new ProviderInfo
            {
                Ukprn = ukprn,
                Name = providerName,
            };

            // act
            Func<Task> func = () => sut.SetViewAsProviderInfoFor(principal, providerInfo);

            // assert
            func.Should().ThrowAsync<ArgumentException>();
        }

        [TestMethod]
        [DataRow("principal1", 12345678, "abc")]
        [DataRow("principal2", 22345678, "def")]
        [DataRow("principal3", 32345678, "ghi")]
        public void SetViewAsProviderInfoFor_WhenInputIsValid_Returns(string principal, int ukprn, string providerName)
        {
            // arrange
            var sut = BuildTestSystem();
            var providerInfo = new ProviderInfo
            {
                Ukprn = ukprn,
                Name = providerName,
            };

            var cacheItem = JsonSerializer.SerializeToUtf8Bytes(providerInfo);
            var responseMessage = new HttpResponseMessage();
            var options = new DistributedCacheEntryOptions();

            var setCacheTask = Task.CompletedTask;

            GetMock(sut.Logger)
               .Setup(x => x.LogInformation($"Setting the view as provider info for '{principal}'"));

            GetMock(sut.Config)
                .Setup(x => x.GetOptions())
                .Returns(options);

            GetMock(sut.Cache)
                .Setup(x => x.SetAsync($"info:{principal}", cacheItem, options, CancellationToken.None))
                .Returns(setCacheTask);

            // act
            var result = sut.SetViewAsProviderInfoFor(principal, providerInfo);

            // assert
            VerifyAllMocks(sut);
            result.Should().Be(setCacheTask);
        }

        #endregion


        #region ClearViewAsProviderInfoFor

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void ClearViewAsProviderInfoFor_WhenPrincipalIsNullOrEmpty_Throws(string principal)
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            Func<Task> func = () => sut.ClearViewAsProviderInfoFor(false, principal);

            // assert
            func.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow("principal 1", false)]
        [DataRow("principal 2", true)]
        [DataRow("principal 3", false)]
        [DataRow("principal 3", true)]
        public void ClearViewAsProviderInfoFor_WhenPrincipalIsValid_Returns(string principal, bool isInfoPrefix)
        {
            // arrange
            var sut = BuildTestSystem();

            var removeCacheTask = Task.CompletedTask;
            string prefix = isInfoPrefix ? "info" : "ukprn";

            GetMock(sut.Logger)
               .Setup(x => x.LogInformation($"Clearing the view as provider details for '{principal}'"));

            GetMock(sut.Cache)
                .Setup(x => x.RemoveAsync($"{prefix}:{principal}", CancellationToken.None))
                .Returns(removeCacheTask);

            // act
            var result = sut.ClearViewAsProviderInfoFor(isInfoPrefix, principal);

            // assert
            VerifyAllMocks(sut);
            result.Should().Be(removeCacheTask);
        }

        #endregion


        internal override ViewAsProviderDetailsCoordinator BuildTestSystem()
        {
            var cache = MakeStrictMock<IDistributedCache>();
            var config = MakeStrictMock<IConfigureCacheStorage>();
            var logger = MakeStrictMock<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>();

            return new ViewAsProviderDetailsCoordinator(cache, config, logger);
        }

        internal override void VerifyAllMocks(ViewAsProviderDetailsCoordinator sut)
        {
            GetMock(sut.Cache).VerifyAll();
            GetMock(sut.Config).VerifyAll();
            GetMock(sut.Logger).VerifyAll();
        }
    }
}