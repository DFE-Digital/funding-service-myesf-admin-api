using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Admin.Services.Implementations.Adapters;
using Pds.Admin.Services.Interfaces.Adapters;
using Pds.Admin.Services.Interfaces.Coordinators;
using Pds.Admin.Services.Models;
using Pds.Services.Common.Interfaces.Adapters;
using Pds.Services.Common.Registration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pds.Admin.Services.Tests.Unit.Adapters
{
    [TestClass, TestCategory("Unit")]
    public sealed class NotifyDetailsAdapterTests :
        MoqTestingTests<NotifyDetailsAdapter, IAdaptNotifyDetails>
    {
        [TestMethod]
        public void SupportsServiceRegistration()
        {
            // arrange / act / assert
            TestSystemSupportsGivenContract<IRequireServiceRegistration>();
        }

        [TestMethod]
        public void ConstructorWithNullCoordinatorThrows()
        {
            // arrange
            var adapter = MakeStrictMock<IAdaptActionResultOperations>();

            // act / assert
            Assert.ThrowsException<ArgumentNullException>(() => new NotifyDetailsAdapter(null, adapter));
        }

        [TestMethod]
        public void ConstructorWithNullSafeOperatorThrows()
        {
            // arrange
            var coordinator = MakeStrictMock<ICoordinateNotifyDetails>();

            // act /assert
            Assert.ThrowsException<ArgumentNullException>(() => new NotifyDetailsAdapter(coordinator, null));
        }

        [TestMethod]
        public async Task GetNotifyDetailsForMeetsExpectation()
        {
            // arrange
            var sut = BuildTestSystem();
            var requestingService = "requestingService";
            var emailMessageType = "emailMessageType";
            var metaData = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("key1", "value1") };

            GetMock(sut.Adapter)
                .Setup(x => x.Run(
                    It.IsAny<Func<Task<HttpResponseMessage>>>(),
                    $"Getting the notify details for '{requestingService}' for type '{emailMessageType}'"))
                .Returns(Task.FromResult(MakeStrictMock<IActionResult>()));

            // act
            var result = await sut.GetNotifyDetailsFor(requestingService, emailMessageType, metaData);

            // assert
            VerifyAllMocks(sut);

            result.Should().BeAssignableTo<IActionResult>();
        }

        [TestMethod]
        public async Task SetNotifyDetailsMeetsExpectation()
        {
            // arrange
            var sut = BuildTestSystem();
            var notifyTemplateDetails = new NotifyTemplateDetails { RequestingService = "RequestingService" };

            GetMock(sut.Adapter)
                .Setup(x => x.Run(
                    It.IsAny<Func<Task<HttpResponseMessage>>>(),
                    $"Setting the notify details for '{notifyTemplateDetails.RequestingService}'"))
                .Returns(Task.FromResult(MakeStrictMock<IActionResult>()));

            // act
            var result = await sut.SetNotifyDetails(notifyTemplateDetails);

            // assert
            VerifyAllMocks(sut);

            result.Should().BeAssignableTo<IActionResult>();
        }

        internal override void VerifyAllMocks(NotifyDetailsAdapter sut)
        {
            GetMock(sut.Coordinator).VerifyAll();
            GetMock(sut.Adapter).VerifyAll();
        }

        internal override NotifyDetailsAdapter BuildTestSystem()
        {
            var coordinator = MakeStrictMock<ICoordinateNotifyDetails>();
            var adapter = MakeStrictMock<IAdaptActionResultOperations>();

            return new NotifyDetailsAdapter(coordinator, adapter);
        }
    }
}
