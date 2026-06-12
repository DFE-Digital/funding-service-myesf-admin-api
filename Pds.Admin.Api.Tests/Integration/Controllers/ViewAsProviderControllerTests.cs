using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Admin.Api.Client.Models;
using Pds.Admin.Api.Controllers;
using Pds.Admin.Services.Implementations.Configuration;
using Pds.Admin.Services.Implementations.Coordinators;
using Pds.Core.Logging;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pds.Admin.Api.Tests.Integration.Controllers
{
    [TestClass, TestCategory("Integration")]
    public class ViewAsProviderControllerTests
    {
        public ViewAsProviderControllerTests()
        {
            Configuration = BuildConfiguration();
        }

        #region GetUkprn

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task GetUkprn_WhenPrincipalIsNullOrEmpty_ReturnsBadRequest(string principal)
        {
            // Arrange
            var logger = Mock.Of<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>(MockBehavior.Strict);

            Mock.Get(logger)
                .Setup(provider => provider.LogInformation($"Getting the view as provider ukprn for '{principal}'"));

            var cacheStorageConfiguration = CreateCacheStorageConfiguration();
            var redisCache = CreateRedisCache(cacheStorageConfiguration);

            var controller = CreateViewAsProviderController(cacheStorageConfiguration, redisCache, logger);

            // Act
            var result = await controller.GetUkprn(principal);

            // Assert
            Mock.Get(logger).Verify();

            result.Should().BeOfType<ActionResult<int>>()
                .Which.Result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task GetUkprn_WhenPrincipalDoesntExist_ReturnsNoContent()
        {
            // Arrange
            const string principal = "non-existing principal";

            var logger = Mock.Of<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>(MockBehavior.Strict);

            Mock.Get(logger)
                .Setup(provider => provider.LogInformation($"Getting the view as provider ukprn for '{principal}'"));

            var cacheStorageConfiguration = CreateCacheStorageConfiguration();
            var redisCache = CreateRedisCache(cacheStorageConfiguration);

            var controller = CreateViewAsProviderController(cacheStorageConfiguration, redisCache, logger);

            // Act
            var result = await controller.GetUkprn(principal);

            // Assert
            Mock.Get(logger).Verify();

            result.Should().BeOfType<ActionResult<int>>()
                .Which.Result.Should().BeOfType<NoContentResult>();
        }

        [TestMethod]
        public async Task GetUkprn_WhenPrincipalExists_ReturnsOk()
        {
            // Arrange
            const string principal = "the requested principal";
            const int providerId = 50000000;

            var logger = Mock.Of<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>(MockBehavior.Strict);

            Mock.Get(logger)
                .Setup(provider => provider.LogInformation($"Getting the view as provider ukprn for '{principal}'"));

            var cacheStorageConfiguration = CreateCacheStorageConfiguration();
            var redisCache = CreateRedisCache(cacheStorageConfiguration);

            var controller = CreateViewAsProviderController(cacheStorageConfiguration, redisCache, logger);

            SetValueInRedisCache(principal, providerId, redisCache, cacheStorageConfiguration);

            // Act
            var result = await controller.GetUkprn(principal);
            RemoveValueFromRedisCache($"ukprn:{principal}", redisCache);

            // Assert
            Mock.Get(logger).Verify();

            result.Should().BeOfType<ActionResult<int>>()
                .Which.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(providerId);
        }

        #endregion


        #region GetProviderInfo

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task GetProviderInfo_WhenPrincipalIsNullOrEmpty_ReturnsBadRequest(string principal)
        {
            // Arrange
            var logger = Mock.Of<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>(MockBehavior.Strict);

            Mock.Get(logger)
                .Setup(provider => provider.LogInformation($"Getting the view as provider info for '{principal}'"));

            var cacheStorageConfiguration = CreateCacheStorageConfiguration();
            var redisCache = CreateRedisCache(cacheStorageConfiguration);

            var controller = CreateViewAsProviderController(cacheStorageConfiguration, redisCache, logger);

            // Act
            var result = await controller.GetProviderInfo(principal);

            // Assert
            Mock.Get(logger).Verify();

            result.Should().BeOfType<ActionResult<ProviderInfo>>()
                .Which.Result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task GetProviderInfo_WhenPrincipalDoesntExist_ReturnsNoContent()
        {
            // Arrange
            string principal = "noprincipal1";

            var logger = Mock.Of<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>(MockBehavior.Strict);

            Mock.Get(logger)
                .Setup(provider => provider.LogInformation($"Getting the view as provider info for '{principal}'"));

            var cacheStorageConfiguration = CreateCacheStorageConfiguration();
            var redisCache = CreateRedisCache(cacheStorageConfiguration);

            var controller = CreateViewAsProviderController(cacheStorageConfiguration, redisCache, logger);

            // Act
            var result = await controller.GetProviderInfo(principal);

            // Assert
            Mock.Get(logger).Verify();

            result.Should().BeOfType<ActionResult<ProviderInfo>>()
                .Which.Result.Should().BeOfType<NoContentResult>();
        }

        [TestMethod]
        public async Task GetProviderInfo_WhenPrincipalExists_ReturnsOk()
        {
            // Arrange
            string principal = "principal1";
            ProviderInfo providerInfo = new ProviderInfo
            {
                Ukprn = 12345678,
                Name = "abc",
            };

            var logger = Mock.Of<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>(MockBehavior.Strict);

            Mock.Get(logger)
                .Setup(provider => provider.LogInformation($"Getting the view as provider info for '{principal}'"));

            var cacheStorageConfiguration = CreateCacheStorageConfiguration();
            var redisCache = CreateRedisCache(cacheStorageConfiguration);

            var controller = CreateViewAsProviderController(cacheStorageConfiguration, redisCache, logger);

            SetValueInRedisCache(principal, providerInfo, redisCache, cacheStorageConfiguration);

            // Act
            var result = await controller.GetProviderInfo(principal);
            RemoveValueFromRedisCache($"info:{principal}", redisCache);

            // Assert
            Mock.Get(logger).Verify();

            result.Should().BeOfType<ActionResult<ProviderInfo>>()
                .Which.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(providerInfo);
        }

        #endregion


        #region SetUkprn

        [TestMethod]
        [DataRow(null, 50000000)]
        [DataRow("", 60000000)]
        [DataRow(" ", 70000000)]
        public async Task SetUkprn_WhenPrincipalNullOrEmpty_ReturnsBadRequest(string principal, int providerID)
        {
            // Arrange
            var logger = Mock.Of<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>(MockBehavior.Strict);

            var cacheStorageConfiguration = CreateCacheStorageConfiguration();
            var redisCache = CreateRedisCache(cacheStorageConfiguration);

            var controller = CreateViewAsProviderController(cacheStorageConfiguration, redisCache, logger);

            // Act
            var result = await controller.SetUkprn(principal, providerID);

            // Assert
            Mock.Get(logger).Verify();
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        [DataRow("principal 1", 0)]
        [DataRow("principal 2", 9999999)]
        [DataRow("principal 3", 100000000)]
        public async Task SetUkprn_WhenProviderIdIsNotBetweenMinAndMax_ReturnsBadRequest(string principal, int providerID)
        {
            // Arrange
            var logger = Mock.Of<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>(MockBehavior.Strict);

            var cacheStorageConfiguration = CreateCacheStorageConfiguration();
            var redisCache = CreateRedisCache(cacheStorageConfiguration);

            var controller = CreateViewAsProviderController(cacheStorageConfiguration, redisCache, logger);

            // Act
            var result = await controller.SetUkprn(principal, providerID);

            // Assert
            Mock.Get(logger).Verify();
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task SetUkprn_WhenInputIsValid_ReturnsOk()
        {
            // arrange
            const string principal = "any old principal...";
            const int providerID = 10000000;

            // Arrange
            var logger = Mock.Of<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>(MockBehavior.Strict);

            Mock.Get(logger)
                .Setup(provider => provider.LogInformation($"Setting the view as provider ukprn for '{principal}'"));

            var cacheStorageConfiguration = CreateCacheStorageConfiguration();
            var redisCache = CreateRedisCache(cacheStorageConfiguration);

            var controller = CreateViewAsProviderController(cacheStorageConfiguration, redisCache, logger);

            // Act
            var result = await controller.SetUkprn(principal, providerID);

            var redisProviderID = GetValueFromRedis(principal, redisCache);
            RemoveValueFromRedisCache($"ukprn:{principal}", redisCache);

            // Assert
            Mock.Get(logger).Verify();

            result.Should().BeOfType<OkResult>();
            redisProviderID.Should().Be(providerID);
        }

        #endregion


        #region SetProviderInfo

        [TestMethod]
        [DataRow(null, 12345678, "abc")]
        [DataRow("", 22345678, "def")]
        [DataRow(" ", 32345678, "ghi")]
        public async Task SetProviderInfo_WhenPrincipalNullOrEmpty_ReturnsBadRequest(string principal, int ukprn, string providerName)
        {
            // Arrange
            var logger = Mock.Of<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>(MockBehavior.Strict);
            var providerInfo = new ProviderInfo
            {
                Ukprn = ukprn,
                Name = providerName,
            };

            var cacheStorageConfiguration = CreateCacheStorageConfiguration();
            var redisCache = CreateRedisCache(cacheStorageConfiguration);

            var controller = CreateViewAsProviderController(cacheStorageConfiguration, redisCache, logger);

            // Act
            var result = await controller.SetProviderInfo(principal, providerInfo);

            // Assert
            Mock.Get(logger).Verify();
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task SetProviderInfo_WhenProviderInfoIsNull_ReturnsBadRequest()
        {
            // Arrange
            var logger = Mock.Of<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>(MockBehavior.Strict);

            var cacheStorageConfiguration = CreateCacheStorageConfiguration();
            var redisCache = CreateRedisCache(cacheStorageConfiguration);

            var controller = CreateViewAsProviderController(cacheStorageConfiguration, redisCache, logger);

            // Act
            var result = await controller.SetProviderInfo("principal1", null);

            // Assert
            Mock.Get(logger).Verify();
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        [DataRow("principal1", 0, "abc")]
        [DataRow("principal2", 9999999, "def")]
        [DataRow("principal3", 100000000, "ghi")]
        [DataRow("principal4", 12345678, null)]
        [DataRow("principal5", 22345678, "")]
        [DataRow("principal6", 32345678, " ")]
        public async Task SetProviderInfo_WhenProviderInfoIsNotValid_ReturnsBadRequest(string principal, int ukprn, string providerName)
        {
            // Arrange
            var logger = Mock.Of<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>(MockBehavior.Strict);
            var providerInfo = new ProviderInfo
            {
                Ukprn = ukprn,
                Name = providerName,
            };

            var cacheStorageConfiguration = CreateCacheStorageConfiguration();
            var redisCache = CreateRedisCache(cacheStorageConfiguration);

            var controller = CreateViewAsProviderController(cacheStorageConfiguration, redisCache, logger);

            // Act
            var result = await controller.SetProviderInfo(principal, providerInfo);

            // Assert
            Mock.Get(logger).Verify();
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task SetProviderInfo_WhenInputIsValid_ReturnsOk()
        {
            // arrange
            var principal = "principal1";
            var providerInfo = new ProviderInfo
            {
                Ukprn = 12345678,
                Name = "abc",
            };

            // Arrange
            var logger = Mock.Of<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>(MockBehavior.Strict);

            Mock.Get(logger)
                .Setup(provider => provider.LogInformation($"Setting the view as provider info for '{principal}'"));

            var cacheStorageConfiguration = CreateCacheStorageConfiguration();
            var redisCache = CreateRedisCache(cacheStorageConfiguration);

            var controller = CreateViewAsProviderController(cacheStorageConfiguration, redisCache, logger);

            // Act
            var result = await controller.SetProviderInfo(principal, providerInfo);

            var redisProviderInfo = GetProviderInfoFromRedis(principal, redisCache);
            RemoveValueFromRedisCache($"info:{principal}", redisCache);

            // Assert
            Mock.Get(logger).Verify();

            result.Should().BeOfType<OkResult>();
            redisProviderInfo.Should().BeEquivalentTo(providerInfo);
        }

        #endregion


        #region DeleteDetails

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task DeleteDetails_WhenPrincipalIsNullOrEmpty_ReturnsBadRequest(string principal)
        {
            // Arrange
            var logger = Mock.Of<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>(MockBehavior.Strict);

            var cacheStorageConfiguration = CreateCacheStorageConfiguration();
            var redisCache = CreateRedisCache(cacheStorageConfiguration);

            var controller = CreateViewAsProviderController(cacheStorageConfiguration, redisCache, logger);

            // Act
            var result = await controller.DeleteDetails(principal);

            // Assert
            Mock.Get(logger).Verify();
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task DeleteDetails_WhenPrincipalHasValue_ReturnsOk()
        {
            // Arrange
            var principal = "the requested principal";
            const int providerId = 10000000;
            var providerInfo = new ProviderInfo
            {
                Ukprn = 12345678,
                Name = "abc",
            };

            var logger = Mock.Of<ILoggerAdapter<ViewAsProviderDetailsCoordinator>>(MockBehavior.Strict);

            Mock.Get(logger)
                .Setup(provider => provider.LogInformation($"Clearing the view as provider details for '{principal}'"));

            var cacheStorageConfiguration = CreateCacheStorageConfiguration();
            var redisCache = CreateRedisCache(cacheStorageConfiguration);

            var controller = CreateViewAsProviderController(cacheStorageConfiguration, redisCache, logger);

            // Act
            SetValueInRedisCache(principal, providerId, redisCache, cacheStorageConfiguration);
            SetValueInRedisCache(principal, providerInfo, redisCache, cacheStorageConfiguration);

            var result = await controller.DeleteDetails(principal);
            var redisProviderID1 = GetValueFromRedis(principal, redisCache);
            var redisProviderID2 = GetProviderInfoFromRedis(principal, redisCache);

            // Assert
            result.Should().BeOfType<OkResult>();
            redisProviderID1.Should().BeNull();
            redisProviderID2.Should().BeNull();
        }

        #endregion

        private IConfiguration Configuration { get; }

        private IConfiguration BuildConfiguration()
            => new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .Build();

        private CacheStorageConfiguration CreateCacheStorageConfiguration()
        {
            var cacheStorageConfig = new CacheStorageConfiguration();
            Configuration.Bind("StorageCache", cacheStorageConfig);

            return cacheStorageConfig;
        }

        private RedisCache CreateRedisCache(CacheStorageConfiguration cacheStorageConfig)
        {
            var redisCacheOptions = new RedisCacheOptions
            {
                Configuration = cacheStorageConfig.ConnectionString
            };

            return new RedisCache(Options.Create(redisCacheOptions));
        }

        private int? GetValueFromRedis(string key, RedisCache redisCache)
        {
            var value = redisCache.Get($"ukprn:{key}");

            return value != null
                ? BitConverter.ToInt32(value)
                : default(int?);
        }

        private ProviderInfo GetProviderInfoFromRedis(string key, RedisCache redisCache)
        {
            var value = redisCache.Get($"info:{key}");

            return value != null
                ? JsonSerializer.Deserialize<ProviderInfo>(Encoding.UTF8.GetString(value))
                : null;
        }

        private void SetValueInRedisCache(
            string key,
            int value,
            RedisCache redisCache,
            CacheStorageConfiguration cacheStorageConfig)
        {
            var valueBytes = BitConverter.GetBytes(value);
            redisCache.Set($"ukprn:{key}", valueBytes, cacheStorageConfig.GetOptions());
        }

        private void SetValueInRedisCache(
            string key,
            ProviderInfo value,
            RedisCache redisCache,
            CacheStorageConfiguration cacheStorageConfig)
        {
            var valueBytes = JsonSerializer.SerializeToUtf8Bytes(value);
            redisCache.Set($"info:{key}", valueBytes, cacheStorageConfig.GetOptions());
        }

        private void RemoveValueFromRedisCache(string key, RedisCache redisCache)
            => redisCache.Remove(key);

        private ViewAsProviderController CreateViewAsProviderController(
            CacheStorageConfiguration cacheStorageConfiguration,
            RedisCache redisCache,
            ILoggerAdapter<ViewAsProviderDetailsCoordinator> logger)
        {
            var viewAsProviderDetailsCoordinator = new ViewAsProviderDetailsCoordinator(
                redisCache,
                cacheStorageConfiguration,
                logger);

            return new ViewAsProviderController(viewAsProviderDetailsCoordinator);
        }
    }
}
