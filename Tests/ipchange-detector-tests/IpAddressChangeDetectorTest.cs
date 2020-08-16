using NUnit.Framework;
using ipchange_detector;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System;
using System.Threading;
using System.IO;

namespace ipchange_detector_tests
{
    public class IpAddressChangeDetectorTest
    {
        [TestCase("1.2.3.4", "6.7.8.9")]
        [TestCase("1.2.3.4", "1.2.3.4")]
        public async Task HasIpAddressChanged_ShouldDetectIsChanged(string oldIpAddress, string newIpAddress)
        {
            // Arrange

            // IHttpClientFactory
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(newIpAddress),
                })
                .Verifiable();

            var client = new HttpClient(mockHttpMessageHandler.Object);
            var expectedUrl = new Uri(IpAddressChangeDetector.IpifyUri);
            client.BaseAddress = expectedUrl;
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // FileSystem
            var mockFileSystem = new MockFileSystem();
            var mockInputFile = new MockFileData(oldIpAddress);
            mockFileSystem.AddFile(IpAddressChangeDetector.PreviousIpAddressFile, mockInputFile);

            // Act
            var sut = new IpAddressChangeDetector(httpClientFactory.Object, mockFileSystem);
            var result = await sut.HasIpAddressChanged();

            // Assert
            Assert.AreEqual(string.Compare(oldIpAddress, newIpAddress) != 0, result.IpAddressHasChanged);
            Assert.AreEqual(oldIpAddress, result.OldIpAddress);
            Assert.AreEqual(newIpAddress, result.NewIpAddress);
        }

        [Test]
        public void HasIpAddressChanged_ShouldThrowException_WhenHttpRequestThrowException()
        {
            // Arrange

            // IHttpClientFactory
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var client = new HttpClient(new ThrowingHandler());
            var expectedUrl = new Uri(IpAddressChangeDetector.IpifyUri);
            client.BaseAddress = expectedUrl;
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // FileSystem
            var mockFileSystem = new MockFileSystem();
            var mockInputFile = new MockFileData("");
            mockFileSystem.AddFile(IpAddressChangeDetector.PreviousIpAddressFile, mockInputFile);

            // Act and Assert
            var sut = new IpAddressChangeDetector(httpClientFactory.Object, mockFileSystem);
            Assert.That(async () => { await sut.HasIpAddressChanged(); }, Throws.Exception.TypeOf<Exception>());
        }

        [TestCase("1.2.3.4", "")]
        public void HasIpAddressChanged_ShouldThrowException_WhenHttpRequestFailsToGetData(string oldIpAddress, string newIpAddress)
        {
            // Arrange

            // IHttpClientFactory
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(newIpAddress),
                })
                .Verifiable();

            var client = new HttpClient(mockHttpMessageHandler.Object);
            var expectedUrl = new Uri(IpAddressChangeDetector.IpifyUri);
            client.BaseAddress = expectedUrl;
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // FileSystem
            var mockFileSystem = new MockFileSystem();
            var mockInputFile = new MockFileData(oldIpAddress);
            mockFileSystem.AddFile(IpAddressChangeDetector.PreviousIpAddressFile, mockInputFile);

            // Act and Assert
            var sut = new IpAddressChangeDetector(httpClientFactory.Object, mockFileSystem);
            Assert.That(async () => { await sut.HasIpAddressChanged(); }, Throws.Exception.TypeOf<InvalidDataException>());
        }
    }

    public class ThrowingHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await Task.FromException<HttpResponseMessage>(new Exception());
        }
    }
}