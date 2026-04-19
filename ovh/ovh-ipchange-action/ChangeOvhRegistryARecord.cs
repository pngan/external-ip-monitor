using System;
using System.Threading.Tasks;
using ipmonitor_interface;
using Serilog;
using Ovh.Api;
using System.Collections.Generic;
using System.Linq;

namespace ipchange_action
{
    public class IpTarget
    {
        public string target { get; set; } = string.Empty;
    }

    public class ChangeOvhRegistryARecord : IIpAddressProcessor
    {
        private readonly ILogger _logger;

        public ChangeOvhRegistryARecord(ILogger logger)
        {
            _logger = logger;
        }

        public async Task ProcessNewIpAddress(string newIpAddress)
        {
            _logger.Information("Processing new IP Address {newIpAddress}: change IP address for DNS A record.", newIpAddress);
            OvhRestClient client = new OvhRestClient(_logger);

            List<string> domains;
            try
            {
                domains = await client.GetAsync<List<string>>("/domain");
                _logger.Information("Retrieved domains from ovh are: {0}", String.Join(",", domains));
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to retrieve domains from OVH. {Exception}", ex);
                return;
            }

            foreach (var domain in domains)
            {
                await UpdateDomain(client, domain, newIpAddress);
            }

            await Task.CompletedTask;
        }

        private async Task UpdateDomain(OvhRestClient client, string domain, string newIpAddress)
        {
            List<long> dnsRecords;
            try
            {
                dnsRecords = await client.GetAsync<List<long>>($"/domain/zone/{domain}/record?fieldType=A");
                if (dnsRecords.Count == 0)
                {
                    _logger.Warning("No DNS A records found for {domain}, skipping.", domain);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to retrieve DNS A records for {domain}. {Exception}", domain, ex);
                return;
            }

            foreach (var recordId in dnsRecords)
            {
                var ipTarget = new IpTarget { target = newIpAddress };
                try
                {
                    await client.PutAsync($"domain/zone/{domain}/record/{recordId}", ipTarget);
                }
                catch (Exception ex)
                {
                    _logger.Error("Unable to set DNS A record {recordId} for {domain} to {IpAddress}. {Exception}", recordId, domain, newIpAddress, ex);
                    return;
                }
            }

            try
            {
                await client.PostAsync($"domain/zone/{domain}/refresh");
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to refresh DNS zone: {domain}. {ex}", domain, ex);
                return;
            }

            _logger.Information("Domain {domain} has been updated to the IP address {newIpAddress}.", domain, newIpAddress);
        }
    }
}