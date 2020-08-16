using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ipmonitor_interface;
using ipmonitor_model;
using System.IO.Abstractions;

namespace ipchange_detector
{
    public class IpAddressChangeDetector : IIpAddressChangeDetector
    {
        private readonly HttpClient _client;
        private readonly IFileSystem _fileSystem;
        public const string PreviousIpAddressFile = @"\ipmon\ip-address.txt";
        public const string IpifyUri = "https://api.ipify.org";

        public IpAddressChangeDetector(IHttpClientFactory httpClientFactory, IFileSystem fileSystem)
        {
            _client = httpClientFactory.CreateClient();
            _fileSystem = fileSystem;
        }

        public async Task<IIpAddressChangedResult> HasIpAddressChanged()
        {
            try
            {
                string currentIpAddress;
                currentIpAddress = await _client.GetStringAsync(IpifyUri);
                if (string.IsNullOrWhiteSpace(currentIpAddress))
                {
                    throw new InvalidDataException("Unable to retrieve external IP Address.");
                }

                string previousIpAddress = string.Empty;
                try
                {
                    previousIpAddress = _fileSystem.File.ReadAllText(PreviousIpAddressFile);
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
                    return new IpAddressChangedResult(false, previousIpAddress, currentIpAddress);
                }

                var file = new FileInfo(PreviousIpAddressFile);
                var fileInfoWrapper = new FileInfoWrapper(_fileSystem, file);
                fileInfoWrapper.Directory.Create(); // If the directory already exists, this method does nothing.

                Console.WriteLine($"IP Address changed from [{previousIpAddress}] to [{currentIpAddress}]");
                _fileSystem.File.WriteAllText(PreviousIpAddressFile, currentIpAddress);
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