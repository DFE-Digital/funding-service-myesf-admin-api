using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Admin.Api.Client.Models;
using Pds.Admin.Api.Controllers;
using Pds.Admin.Services.Interfaces.Coordinators;
using System;
using System.Threading.Tasks;

namespace Pds.Admin.Api.Tests.Unit.Controllers
{
    [TestClass, TestCategory("Unit")]
    public sealed class ViewAsProviderControllerTests :
        MoqTestingTests<ViewAsProviderController>
    {
        [TestMethod]
        public void ConstructorFailsWithNullController()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => new ViewAsProviderController(null));
        }

        #region GetUkprn

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task GetUkprn_WhenPrincipalNullOrEmpty_ReturnsBadRequest(string principal)
        {
            // Arrange
            var controller = BuildTestSystem();

            // Act
            var result = await controller.GetUkprn(principal);

            // Assert
            VerifyAllMocks(controller);

            result.Should().BeOfType<ActionResult<int>>()
                .Which.Result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task GetUkprn_WhenPrincipalDoesntExist_ReturnsNoContent()
        {
            // Arrange
            string testPrincipal = "any old principal...";

            var controller = BuildTestSystem();

            GetMock(controller.Coordinator)
                .Setup(x => x.GetViewAsProviderUkprnFor(testPrincipal))
                .Returns(Task.FromResult(null as int?));

            // Act
            var result = await controller.GetUkprn(testPrincipal);

            // Assert
            VerifyAllMocks(controller);

            result.Should().BeOfType<ActionResult<int>>()
                .Which.Result.Should().BeOfType<NoContentResult>();
        }

        [TestMethod]
        public async Task GetUkprn_WhenPrincipalExists_ReturnsOk()
        {
            // Arrange
            string testPrincipal = "any old principal...";
            int? testProviderID = 50000000;

            var controller = BuildTestSystem();

            GetMock(controller.Coordinator)
                .Setup(x => x.GetViewAsProviderUkprnFor(testPrincipal))
                .Returns(Task.FromResult(testProviderID));

            // Act
            var result = await controller.GetUkprn(testPrincipal);

            // Assert
            VerifyAllMocks(controller);

            result.Should().BeOfType<ActionResult<int>>()
                .Which.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(testProviderID);
        }

        #endregion


        #region GetProviderInfo

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task GetProviderInfo_WhenPrincipalNullOrEmpty_ReturnsBadRequest(string principal)
        {
            // Arrange
            var controller = BuildTestSystem();

            // Act
            var result = await controller.GetProviderInfo(principal);

            // Assert
            VerifyAllMocks(controller);

            result.Should().BeOfType<ActionResult<ProviderInfo>>()
                .Which.Result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task GetProviderInfo_WhenPrincipalDoesntExist_ReturnsNoContent()
        {
            // Arrange
            var principal = "noprincipal1";

            var controller = BuildTestSystem();

            GetMock(controller.Coordinator)
                .Setup(x => x.GetViewAsProviderInfoFor(principal))
                .Returns(Task.FromResult(null as ProviderInfo));

            // Act
            var result = await controller.GetProviderInfo(principal);

            // Assert
            VerifyAllMocks(controller);

            result.Should().BeOfType<ActionResult<ProviderInfo>>()
                .Which.Result.Should().BeOfType<NoContentResult>();
        }

        [TestMethod]
        public async Task GetProviderInfo_WhenPrincipalExists_ReturnsOk()
        {
            // Arrange
            var principal = "principal1";
            var providerInfo = new ProviderInfo
            {
                Ukprn = 12345678,
                Name = "abc",
            };

            var controller = BuildTestSystem();

            GetMock(controller.Coordinator)
                .Setup(x => x.GetViewAsProviderInfoFor(principal))
                .Returns(Task.FromResult(providerInfo));

            // Act
            var result = await controller.GetProviderInfo(principal);

            // Assert
            VerifyAllMocks(controller);

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
            var controller = BuildTestSystem();

            // Act
            var result = await controller.SetUkprn(principal, providerID);

            // Assert
            VerifyAllMocks(controller);
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        [DataRow("principal 1", 0)]
        [DataRow("principal 2", 9999999)]
        [DataRow("principal 3", 100000000)]
        public async Task SetUkprn_WhenProviderIdIsNotBetweenMinAndMax_ReturnsBadRequest(string principal, int providerID)
        {
            // Arrange
            var controller = BuildTestSystem();

            // Act
            var result = await controller.SetUkprn(principal, providerID);

            // Assert
            VerifyAllMocks(controller);
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task SetUkprn_WhenInputIsValid_ReturnsOk()
        {
            // arrange
            const string principal = "any old principal...";
            const int providerID = 10000000;

            var sut = BuildTestSystem();

            GetMock(sut.Coordinator)
                .Setup(x => x.SetViewAsProviderUkprnFor(principal, providerID))
                .Returns(Task.CompletedTask);

            // act
            var result = await sut.SetUkprn(principal, providerID);

            // assert
            VerifyAllMocks(sut);
            result.Should().BeOfType<OkResult>();
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
            var controller = BuildTestSystem();
            var providerInfo = new ProviderInfo
            {
                Ukprn = ukprn,
                Name = providerName,
            };

            // Act
            var result = await controller.SetProviderInfo(principal, providerInfo);

            // Assert
            VerifyAllMocks(controller);
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task SetProviderInfo_WhenPrincipalInfoIsNull_ReturnsBadRequest()
        {
            // Arrange
            var controller = BuildTestSystem();

            // Act
            var result = await controller.SetProviderInfo("principal1", null);

            // Assert
            VerifyAllMocks(controller);
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
            var controller = BuildTestSystem();
            var providerInfo = new ProviderInfo
            {
                Ukprn = ukprn,
                Name = providerName,
            };

            // Act
            var result = await controller.SetProviderInfo(principal, providerInfo);

            // Assert
            VerifyAllMocks(controller);
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

            var sut = BuildTestSystem();

            GetMock(sut.Coordinator)
                .Setup(x => x.SetViewAsProviderInfoFor(principal, providerInfo))
                .Returns(Task.CompletedTask);

            // act
            var result = await sut.SetProviderInfo(principal, providerInfo);

            // assert
            VerifyAllMocks(sut);
            result.Should().BeOfType<OkResult>();
        }

        #endregion


        #region DeleteDetails

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task DeleteDetails_WhenPrincipalIsNullOrEmpty_ReturnsBadRequest(string principal)
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            var result = await sut.DeleteDetails(principal);

            // assert
            VerifyAllMocks(sut);
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task DeleteDetails_WhenPrincipalHasValue_ReturnsOk()
        {
            // arrange
            const string principal = "any old principal...";

            var sut = BuildTestSystem();

            GetMock(sut.Coordinator)
                .Setup(x => x.ClearViewAsProviderInfoFor(false, principal))
                .Returns(Task.CompletedTask);
            GetMock(sut.Coordinator)
                .Setup(x => x.ClearViewAsProviderInfoFor(true, principal))
                .Returns(Task.CompletedTask);

            // act
            var result = await sut.DeleteDetails(principal);

            // assert
            VerifyAllMocks(sut);
            result.Should().BeOfType<OkResult>();
        }

        #endregion

        internal override ViewAsProviderController BuildTestSystem()
        {
            var coordinator = MakeStrictMock<ICoordinateViewAsProviderDetails>();
            return new ViewAsProviderController(coordinator);
        }

        internal override void VerifyAllMocks(ViewAsProviderController sut)
            => GetMock(sut.Coordinator).VerifyAll();
    }
}