using NUnit.Framework;
using ipchange_action;
using Moq;
using ipmonitor_interface;
using System.Threading.Tasks;

namespace ipchange_action_tests
{
    public class IpAddressProcessorEngineTest
    {
        [Test]
        public async Task VerifyEachRegisteredProcessorIsCalled()
        {
            // Arrange
            var processor1Mock = new Mock<IIpAddressProcessor>();
            processor1Mock.Setup(p => p.ProcessNewIpAddress("", ""));

            var processor2Mock = new Mock<IIpAddressProcessor>();
            processor2Mock.Setup(p => p.ProcessNewIpAddress("", ""));

            string testAddress = "a.com";

            // Act
            var sut = new IpAddressProcessorEngine();
            sut.RegisterAddressProcessors(new[] { processor1Mock.Object, processor2Mock.Object});
            await sut.ProcessNewIpAddress("", testAddress);

            // Assert
            processor1Mock.Verify(p => p.ProcessNewIpAddress("", testAddress), Times.Once());
            processor2Mock.Verify(p => p.ProcessNewIpAddress("", testAddress), Times.Once());
        }
    }
}