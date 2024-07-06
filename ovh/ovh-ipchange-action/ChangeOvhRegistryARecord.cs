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

        public async Task ProcessNewIpAddress(string dnsARecord, string newIpAddress)
        {
            _logger.Information("Processing new IP Address {newIpAddress}: change IP address for DNS A record.", newIpAddress);
            OvhRestClient client = new OvhRestClient(_logger);

            List<string> domain;
            string? aRecord = null;
            try
            {
                domain = await client.GetAsync<List<string>>("/domain");

                _logger.Information("Retrieved domains from ovh are: {0}", String.Join(",", domain));

                aRecord = domain.FirstOrDefault(d => string.Equals(d, dnsARecord, StringComparison.OrdinalIgnoreCase));

                if (aRecord is null)
                {
                    _logger.Error("Unable to retrieve domain {dnsARecord} from Ovh", dnsARecord);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to retrieve domain. {Exception}", ex);
                return;
            }

            _logger.Information("Domain {aRecord} has been retrieved.", aRecord);


            List<long> dnsRecords;
            try
            {
                dnsRecords = await client.GetAsync<List<long>>($"/domain/zone/{aRecord}/record?fieldType=A");
                if (dnsRecords.Count != 1)
                {
                    _logger.Error("Unable to retrieve DNS A record for {aRecord}", aRecord);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to retrieve DNS A record. {Exception}", ex);
                return;
            }


            var ipTarget = new IpTarget { target = newIpAddress };
            try
            {
                await client.PutAsync($"domain/zone/{aRecord}/record/{dnsRecords[0]}", ipTarget);
            }
            catch(Exception ex)
            {
                _logger.Error("Unable to set DNS A record, to IpAddress {IpAddress}. {Exception}", newIpAddress, ex);
                return;
            }

            try
            {
                await client.PostAsync($"domain/zone/{aRecord}/refresh");
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to refresh DNS zone: {aRecord}. {ex}", aRecord, ex);
                return;
            }

            _logger.Information("Domain {aRecord} has been updated to the IP address {newIpAddress}.", aRecord, newIpAddress);

            await Task.CompletedTask;
        }
    }
}