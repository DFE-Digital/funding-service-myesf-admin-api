using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Admin.Api.Controllers;
using Pds.Admin.Services.Interfaces.Adapters;
using Pds.Admin.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pds.Admin.Api.Tests.Unit.Controllers
{
    [TestClass, TestCategory("Unit")]
    public sealed class NotifyControllerTests :
        MoqTestingTests<NotifyController>
    {
        [TestMethod]
        public void ConstructorFailsWithNullAdpater()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => new NotifyController(null));
        }

        [TestMethod]
        public async Task GetDetailsMeetsExpectation()
        {
            // arrange
            const string requestingService = "any service";
            const string emailMessageType = "any message type";
            var metaData = new Dictionary<string, string> { { "key1", "value1" } };

            var sut = BuildTestSystem();

            GetMock(sut.Adapter)
                .Setup(x => x.GetNotifyDetailsFor(requestingService, emailMessageType, It.IsAny<List<KeyValuePair<string, string>>>()))
                .ReturnsAsync(new OkObjectResult(null));

            // act
            var result = await sut.GetDetails(requestingService, emailMessageType, metaData);

            // assert
            VerifyAllMocks(sut);

            result.Should().BeAssignableTo<IActionResult>();
        }

        [TestMethod]
        public async Task SetDetailsMeetsExpectation()
        {
            // arrange
            var notifyTemplateDetails = new NotifyTemplateDetails();

            var sut = BuildTestSystem();

            GetMock(sut.Adapter)
                .Setup(x => x.SetNotifyDetails(notifyTemplateDetails))
                .ReturnsAsync(new OkObjectResult(null));

            // act
            var result = await sut.SetDetails(notifyTemplateDetails);

            // assert
            VerifyAllMocks(sut);

            result.Should().BeAssignableTo<IActionResult>();
        }

        internal override NotifyController BuildTestSystem()
        {
            var adapter = MakeStrictMock<IAdaptNotifyDetails>();

            return new NotifyController(adapter);
        }

        internal override void VerifyAllMocks(NotifyController sut)
        {
            GetMock(sut.Adapter).VerifyAll();
        }
    }
}
