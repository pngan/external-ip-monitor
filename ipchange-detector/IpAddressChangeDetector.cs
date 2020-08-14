using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ipmonitor_interface;
using ipmonitor_model;

namespace ipchange_detector
{
    public class IpAddressChangeDetector : IIpAddressChangeDetector
    {
        readonly HttpClient _client;

        public IpAddressChangeDetector(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }


        public async Task<IIpAddressChangedResult> HasIpAddressChanged()
        {
            try
            {
                string currentIpAddress;
                try
                {
                    currentIpAddress = await _client.GetStringAsync("https://api.ipify.org");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to retrieve external IP Address: [{ex}]");
                    return new IpAddressChangedResult();
                }

                if (string.IsNullOrWhiteSpace(currentIpAddress))
                {
                    Console.WriteLine("Unable to retrieve external IP Address.");
                    return new IpAddressChangedResult();
                }

                string previousIpAddress = string.Empty;
                var fileName = @"/ipmon/ip-address.txt";
                try
                {
                    previousIpAddress = File.ReadAllText(fileName);
                }
                catch
                {
                    return new IpAddressChangedResult();
                }


                if (!string.IsNullOrWhiteSpace(previousIpAddress)
                    && previousIpAddress.Equals(currentIpAddress, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"IP Address remains unchanged from [{currentIpAddress}]");

                    // IP Address unchanged, nothing to do, exit
                    return new IpAddressChangedResult();
                }

                var file = new FileInfo(fileName);
                file.Directory.Create(); // If the directory already exists, this method does nothing.

                Console.WriteLine($"IP Address changed from [{previousIpAddress}] to [{currentIpAddress}]");
                File.WriteAllText(fileName, currentIpAddress);
                return new IpAddressChangedResult(true, previousIpAddress, currentIpAddress);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return new IpAddressChangedResult();
        }
    }
}