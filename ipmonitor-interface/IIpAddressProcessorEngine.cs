using System.Threading.Tasks;

namespace ipmonitor_interface
{
    public interface IIpAddressProcessorEngine
    {
        Task ProcessNewIpAddress(string dnsARecord, string newIpAddress);

        void RegisterAddressProcessors(IIpAddressProcessor[] ipAddressProcessors);
    }
}