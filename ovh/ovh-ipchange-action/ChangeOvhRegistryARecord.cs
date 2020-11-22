using System;
using System.Threading.Tasks;
using ipmonitor_interface;
using Serilog;
using Ovh.Api;
using System.Collections.Generic;

namespace ipchange_action
{
    public class IpTarget
    {
        public string target { get; set; }
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

            List<string> domain;
            try
            {
                domain = await client.GetAsync<List<string>>("/domain");
                if (domain.Count == 0)
                {
                    _logger.Error("Unable to retrieve domain");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to retrieve domain. {Exception}", ex);
                return;
            }

            List<long> dnsRecords;
            try
            {
                dnsRecords = await client.GetAsync<List<long>>($"/domain/zone/{domain[0]}/record?fieldType=A");
                if (dnsRecords.Count != 1)
                {
                    _logger.Error("Unable to retrieve DNS A record");
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
                await client.PutAsync($"domain/zone/{domain[0]}/record/{dnsRecords[0]}", ipTarget);
            }
            catch(Exception ex)
            {
                _logger.Error("Unable to set DNS A record, to IpAddress {IpAddress}. {Exception}", newIpAddress, ex);
                return;
            }

            try
            {
                await client.PostAsync($"domain/zone/{domain[0]}/refresh");
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to refresh DNS zone: {domain}. {Exception}", domain[0], ex);
                return;
            }

            await Task.CompletedTask;
        }
    }
}