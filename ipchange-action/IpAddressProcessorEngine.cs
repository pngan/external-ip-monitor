using System.Collections.Generic;
using System.Threading.Tasks;
using ipmonitor_interface;

namespace ipchange_action
{
    public class IpAddressProcessorEngine : IIpAddressProcessorEngine
    {
        private readonly List<IIpAddressProcessor> _processors = new List<IIpAddressProcessor>();
        public async Task ProcessNewIpAddress(string dnsARecord, string newIpAddress)
        {
            foreach (var addressProcessor in _processors)
            {
                await addressProcessor.ProcessNewIpAddress(dnsARecord, newIpAddress);
            }
        }

        public void RegisterAddressProcessors(IIpAddressProcessor[] ipAddressProcessors)
        {
            _processors.AddRange(ipAddressProcessors);
        }
    }
}