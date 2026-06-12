using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Table;
using Pds.Admin.Services.Implementations.Coordinators;
using Pds.Admin.Services.Interfaces.Coordinators;
using Pds.Admin.Services.Models;
using Pds.Core.AzureStorage.Interfaces;
using Pds.Services.Common.Faults;
using Pds.Services.Common.Interfaces.Factories;
using Pds.Services.Common.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using It = Moq.It;

namespace Pds.Admin.Services.Tests.Unit.Coordinators
{
    [TestClass, TestCategory("Unit")]
    public sealed class NotifyDetailsCoordinatorTests :
        MoqTestingTests<NotifyDetailsCoordinator, ICoordinateNotifyDetails>
    {
        [TestMethod]
        public void SupportsServiceRegistration()
        {
            // arrange / act / assert
            TestSystemSupportsGivenContract<IRequireServiceRegistration>();
        }

        [TestMethod]
        public void ConstructorWithNullRepoThrows()
        {
            // arrange
            var response = MakeStrictMock<ICreateHttpResponseMessages>();

            // act / assert
            Assert.ThrowsException<ArgumentNullException>(() => new NotifyDetailsCoordinator(null, response));
        }

        [TestMethod]
        public void ConstructorWithNullResponseFactoryThrows()
        {
            // arrange
            var repo = MakeStrictMock<IAzureTableStorageRepository<NotifyServiceTemplateDetails>>();

            // act / assert
            Assert.ThrowsException<ArgumentNullException>(() => new NotifyDetailsCoordinator(repo, null));
        }

        [TestMethod]
        [DataRow(null, "data")]
        [DataRow("", "data")]
        [DataRow("data", null)]
        [DataRow("data", "")]
        public async Task GetNotifyDetailsForThrows(string requestingService, string emailMessageType)
        {
            // arrange
            var sut = BuildTestSystem();

            // act / assert
            await Assert.ThrowsExceptionAsync<MalformedRequestException>(() => sut.GetNotifyDetailsFor(requestingService, emailMessageType, null));
        }

        [TestMethod]
        public async Task GetNotifyDetailsForMeetsExpectation()
        {
            // arrange
            var requestingService = "requestingService";
            var emailMessageType = "emailMessageType";
            List<KeyValuePair<string, string>> metaData = null;

            var sut = BuildTestSystem();
            var expectedFilterString = $"(PartitionKey eq '{emailMessageType}') and (RequestingService eq '{requestingService}')";
            var expectedTableResults = new List<NotifyServiceTemplateDetails>
            {
                new NotifyServiceTemplateDetails
                    { PartitionKey = emailMessageType, RowKey = "row1", RequestingService = requestingService }
            };

            GetMock(sut.NotifyTableStorageRepository)
                .Setup(x => x.GetMany(
                    It.Is<TableQuery<NotifyServiceTemplateDetails>>(op => op.FilterString == expectedFilterString)))
                .Returns(Task.FromResult(expectedTableResults));

            GetMock(sut.Response)
                .Setup(x => x.Create(HttpStatusCode.OK, It.Is<NotifyTemplateDetails>(n => AreEquivalent(n, expectedTableResults.First()))))
                .Returns(Task.FromResult(new HttpResponseMessage()));

            // act
            var result = await sut.GetNotifyDetailsFor(requestingService, emailMessageType, metaData);

            // assert
            VerifyAllMocks(sut);
            result.Should().BeAssignableTo<HttpResponseMessage>();
        }

        [TestMethod]
        [DataRow(new[] { "ID" }, new[] { "0003" }, new[] { "row1" })]
        [DataRow(new[] { "ID", "Value", "NotifyApiKeySecretName" }, new[] { "0003", "20000", "NotifyApiKeySecretName1" }, new[] { "row1" })]
        [DataRow(new[] { "ID", "Value", "NotifyApiKeySecretName" }, new[] { "0004", "50000", "NotifyApiKeySecretName3" }, new[] { "row3" })]
        [DataRow(new[] { "" }, new[] { "" }, new[] { "row1" })]
        public async Task GetNotifyDetailsForWithMetaDataFilterMeetsExpectation(string[] keys, string[] values, string[] expectedRows)
        {
            // arrange
            var requestingService = "requestingService";
            var emailMessageType = "emailMessageType";

            var metaData = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < keys.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(keys[i]))
                {
                    metaData.Add(new KeyValuePair<string, string>(keys[i], values[i]));
                }
            }

            var sut = BuildTestSystem();
            var expectedTableResults = new List<NotifyServiceTemplateDetails>
            {
                new NotifyServiceTemplateDetails
                {
                    RowKey = "row1", Metadata = @"{""ID"":""0003"", ""Value"":""20000"", ""NotifyApiKeySecretName"":""NotifyApiKeySecretName1""}",
                    PartitionKey = emailMessageType, RequestingService = requestingService
                },
                new NotifyServiceTemplateDetails
                {
                    RowKey = "row2", Metadata = @"{""ID"":""0003"", ""Value"":""30000"", ""NotifyApiKeySecretName"":""NotifyApiKeySecretName2""}",
                    PartitionKey = emailMessageType, RequestingService = requestingService
                },
                new NotifyServiceTemplateDetails
                {
                    RowKey = "row3", Metadata = @"{""ID"":""0004"", ""Value"":""50000"", ""NotifyApiKeySecretName"":""NotifyApiKeySecretName3""}",
                    PartitionKey = emailMessageType, RequestingService = requestingService,
                },
                new NotifyServiceTemplateDetails
                {
                    RowKey = "row4", Metadata = null, PartitionKey = emailMessageType,
                    RequestingService = requestingService
                },
                new NotifyServiceTemplateDetails
                {
                    RowKey = "row5", Metadata = @"{""ProdID"":""0003"", ""ProdValue"":""20000"", ""ProdNotifyApiKeySecretName"":""NotifyApiKeySecretName5""}",
                    PartitionKey = emailMessageType, RequestingService = requestingService
                },
            };
            var expectedFilteredResult = expectedTableResults.FirstOrDefault(r => expectedRows.Contains(r.RowKey));

            GetMock(sut.NotifyTableStorageRepository)
                .Setup(x => x.GetMany(
                    It.IsAny<TableQuery<NotifyServiceTemplateDetails>>()))
                .Returns(Task.FromResult(expectedTableResults));

            GetMock(sut.Response)
                .Setup(x => x.Create(HttpStatusCode.OK, It.Is<NotifyTemplateDetails>(n => AreEquivalent(n, expectedFilteredResult))))
                .Returns(Task.FromResult(new HttpResponseMessage()));

            // act
            var result = await sut.GetNotifyDetailsFor(requestingService, emailMessageType, metaData);

            // assert
            VerifyAllMocks(sut);
            result.Should().BeAssignableTo<HttpResponseMessage>();
        }

        [TestMethod]
        public async Task GetNotifyDetailsForNoResultReturnsCorrectResponse()
        {
            // arrange
            var requestingService = "requestingService";
            var emailMessageType = "emailMessageType";
            List<KeyValuePair<string, string>> metaData = null;

            var sut = BuildTestSystem();
            var expectedTableResults = new List<NotifyServiceTemplateDetails>();

            GetMock(sut.NotifyTableStorageRepository)
                .Setup(x => x.GetMany(
                    It.IsAny<TableQuery<NotifyServiceTemplateDetails>>()))
                .Returns(Task.FromResult(expectedTableResults));

            GetMock(sut.Response)
                .Setup(x => x.Create(HttpStatusCode.NoContent, "Content not found."))
                .Returns(new HttpResponseMessage());

            // act
            var result = await sut.GetNotifyDetailsFor(requestingService, emailMessageType, metaData);

            // assert
            VerifyAllMocks(sut);
            result.Should().BeAssignableTo<HttpResponseMessage>();
        }

        [TestMethod]
        public async Task SetNotifyDetailsWithNullDetailsThrows()
        {
            // arrange
            var sut = BuildTestSystem();

            // act / assert
            await Assert.ThrowsExceptionAsync<MalformedRequestException>(() => sut.SetNotifyDetails(null));
        }

        [TestMethod]
        public async Task SetNotifyDetailsMeetsExpectation()
        {
            // arrange
            var sut = BuildTestSystem();

            var notifyTemplateDetails = new NotifyTemplateDetails { RequestingService = "service", EmailMessageType = "Message type" };
            var responseMessage = new HttpResponseMessage();

            GetMock(sut.NotifyTableStorageRepository)
               .Setup(x => x.Insert(It.IsAny<List<NotifyServiceTemplateDetails>>()))
               .Returns(Task.CompletedTask);

            GetMock(sut.Response)
                .Setup(x => x.Create(HttpStatusCode.Created, string.Empty))
                .Returns(responseMessage);

            // act
            var result = await sut.SetNotifyDetails(notifyTemplateDetails);

            // assert
            VerifyAllMocks(sut);

            result.Should().BeAssignableTo<HttpResponseMessage>();
        }

        internal override NotifyDetailsCoordinator BuildTestSystem()
        {
            var notifyTableStorageRepository = MakeStrictMock<IAzureTableStorageRepository<NotifyServiceTemplateDetails>>();
            var response = MakeStrictMock<ICreateHttpResponseMessages>();

            return new NotifyDetailsCoordinator(notifyTableStorageRepository, response);
        }

        internal override void VerifyAllMocks(NotifyDetailsCoordinator sut)
        {
            GetMock(sut.NotifyTableStorageRepository).VerifyAll();
            GetMock(sut.Response).VerifyAll();
        }

        internal bool AreEquivalentLists(IEnumerable<NotifyTemplateDetails> actual, IEnumerable<NotifyServiceTemplateDetails> expected)
        {
            return actual.Count() == expected.Count()
                   && actual.All(a => expected.Any(e =>
                       e.RowKey.Equals(a.RowKey, StringComparison.InvariantCultureIgnoreCase)));
        }

        internal bool AreEquivalent(NotifyTemplateDetails actual, NotifyServiceTemplateDetails expected)
        {
            return actual.EmailMessageType.Equals(expected.PartitionKey, StringComparison.InvariantCultureIgnoreCase)
                   && actual.RequestingService.Equals(expected.RequestingService, StringComparison.InvariantCultureIgnoreCase)
                   && actual.RowKey.Equals(expected.RowKey, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}