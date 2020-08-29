using System;
using System.Threading.Tasks;
using ipmonitor_interface;
using Serilog;

namespace ipchange_action
{
    public class ChangeOvhRegistryARecord : IIpAddressProcessor
    {
        private readonly ILogger _logger;

        public ChangeOvhRegistryARecord(ILogger logger)
        {
            _logger = logger;
        }

        public async Task ProcessNewIpAddress(
            string newIpAddress)
        {
            _logger.Information("Processing new IP Address {newIpAddress}", newIpAddress);
            await Task.CompletedTask;
        }
    }
}