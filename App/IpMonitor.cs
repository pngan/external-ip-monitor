using System;
using System.Threading.Tasks;
using ipchange_action;
using ipchange_detector;
using ipmonitor_interface;
using Microsoft.Extensions.Configuration;

namespace NetCore.Docker
{
    public class IpMonitor : IIpMonitor
    {
        private const string AppSettingsFile = "appsettings.json";
        private readonly int CheckIntervalInSeconds;
        private readonly IIpAddressChangeDetector _addressChangeDetector;
        private readonly IIpAddressProcessorEngine _addressProcessorEngine;

        public IpMonitor(
            IIpAddressChangeDetector addressChangeDetector,
            IIpAddressProcessorEngine addressProcessorEngine,
            IIpAddressProcessor[] addressProcessors)
        {
            _addressChangeDetector = addressChangeDetector;
            _addressProcessorEngine = addressProcessorEngine;
            _addressProcessorEngine.RegisterAddressProcessors(addressProcessors);

            IConfiguration config = new ConfigurationBuilder()
              .AddJsonFile(AppSettingsFile, true, true)
              .Build();

            try
            {
                CheckIntervalInSeconds = int.Parse(config["pollIntervalInSeconds"]);
            }
            catch
            {
                Console.WriteLine("Unable to read integer value for 'pollIntervalInSeconds' from appsettings.json.");
            }
        }

        public async Task<int> RunIpMonitor()
        {
            try
            {
                while (true)
                {
                    await CheckIpAddressChange();
                    await Task.Delay(TimeSpan.FromSeconds(CheckIntervalInSeconds));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occurred [{e}]");
                return -1;
            }
        }

        public async Task CheckIpAddressChange()
        {
            var ipChangeResult = await _addressChangeDetector.HasIpAddressChanged();
            if (ipChangeResult.IpAddressHasChanged)
                await _addressProcessorEngine.ProcessNewIpAddress(ipChangeResult.NewIpAddress);
        }
    }
}