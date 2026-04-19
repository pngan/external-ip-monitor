using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using ipchange_action;
using ipchange_detector;
using ipmonitor_interface;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace NetCore.Docker
{
    public class IpMonitor : IIpMonitor
    {
        private const string AppSettingsFile = "appsettings.json";
        private readonly int CheckIntervalInSeconds;
        private readonly bool AlwaysUpdateIpAddress;
        private readonly List<string> DnsARecords;
        private readonly IIpAddressChangeDetector _addressChangeDetector;
        private readonly IIpAddressProcessorEngine _addressProcessorEngine;
        private readonly ILogger _logger;

        public IpMonitor(
            IIpAddressChangeDetector addressChangeDetector,
            IIpAddressProcessorEngine addressProcessorEngine,
            IIpAddressProcessor[] addressProcessors,
            ILogger logger)
        {
            _addressChangeDetector = addressChangeDetector;
            _addressProcessorEngine = addressProcessorEngine;
            _addressProcessorEngine.RegisterAddressProcessors(addressProcessors);
            _logger = logger;

            _logger.Information("Reading configuration from: {AppSettingsFile}", AppSettingsFile);
            IConfiguration config = new ConfigurationBuilder()
              .AddJsonFile(AppSettingsFile, true, true)
              .Build();

            try
            {
                CheckIntervalInSeconds = int.Parse(config["pollIntervalInSeconds"]);
                _logger.Information("Config: pollIntervalInSeconds = {PollingInterval} seconds", CheckIntervalInSeconds);
                AlwaysUpdateIpAddress = bool.Parse(config["alwaysUpdateIpAddress"]);
                _logger.Information("Config: alwaysUpdateIpAddress = {AlwaysUpdateIpAddress}; Force address update on every address check", AlwaysUpdateIpAddress);

                var configuredDomains = config.GetSection("domains").Get<List<string>>() ?? new List<string>();
                if (configuredDomains.Count > 0)
                {
                    DnsARecords = configuredDomains;
                }
                else
                {
                    var envRecord = Environment.GetEnvironmentVariable("DNSARECORD") ?? string.Empty;
                    DnsARecords = string.IsNullOrWhiteSpace(envRecord) ? new List<string>() : new List<string> { envRecord };
                }

                _logger.Information("Config: domains = [{DnsARecords}]; The DNS A Records to update", string.Join(", ", DnsARecords));
            }
            catch
            {
                DnsARecords = new List<string>();
                _logger.Error("Unable to read integer value for 'pollIntervalInSeconds' from: {AppSettingsFile}", AppSettingsFile);
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
                _logger.Error("Exception occurred {exception}", e);
                return -1;
            }
        }

        public async Task CheckIpAddressChange()
        {
            var ipChangeResult = await _addressChangeDetector.HasIpAddressChanged();
            if (ipChangeResult.IpAddressHasChanged || AlwaysUpdateIpAddress)
            {
                foreach (var domain in DnsARecords)
                {
                    await _addressProcessorEngine.ProcessNewIpAddress(domain, ipChangeResult.NewIpAddress);
                }
            }
        }
    }
}