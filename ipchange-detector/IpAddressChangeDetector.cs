using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ipmonitor_interface;
using ipmonitor_model;
using System.IO.Abstractions;
using Serilog;

namespace ipchange_detector
{
    public class IpAddressChangeDetector : IIpAddressChangeDetector
    {
        private readonly HttpClient _client;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        public const string PreviousIpAddressFile = @"ipmon/ip-address.txt";
        public const string IpifyUri = "https://api.ipify.org";

        public IpAddressChangeDetector(IHttpClientFactory httpClientFactory, IFileSystem fileSystem, ILogger logger)
        {
            _client = httpClientFactory.CreateClient();
            _fileSystem = fileSystem;
            _logger = logger;
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

                if (_fileSystem.File.Exists(PreviousIpAddressFile))
                {
                    try
                    {
                        previousIpAddress = _fileSystem.File.ReadAllText(PreviousIpAddressFile);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Unable to read previous ip address from file {dataFile}", PreviousIpAddressFile);
                        return new IpAddressChangedResult();
                    }

                    if (!string.IsNullOrWhiteSpace(previousIpAddress)
                        && previousIpAddress.Equals(currentIpAddress, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.Information("External IP Address remains unchanged from {currentIpAddress}", currentIpAddress);

                        // IP Address unchanged, nothing to do, exit
                        return new IpAddressChangedResult(false, previousIpAddress, currentIpAddress);
                    }
                }

                var file = new FileInfo(PreviousIpAddressFile);
                var fileInfoWrapper = new FileInfoWrapper(_fileSystem, file);
                fileInfoWrapper.Directory.Create(); // If the directory already exists, this method does nothing.

                _logger.Information("External IP Address changed from {previousIpAddress} to {currentIpAddress}", previousIpAddress, currentIpAddress);
                _fileSystem.File.WriteAllText(PreviousIpAddressFile, currentIpAddress);
                return new IpAddressChangedResult(true, previousIpAddress, currentIpAddress);
            }
            catch (HttpRequestException e)
            {
                _logger.Error(e, "Unable to retrieve external IP Address from {endPoint}", IpifyUri);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error occured while determining if address had changed");
                throw;
            }

            return new IpAddressChangedResult();
        }
    }
}