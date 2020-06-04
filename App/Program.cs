using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace NetCore.Docker
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        static async Task<int> Main(string[] args)
        {
            try
            {
                string currentIpAddress;
                try
                {
                    currentIpAddress = await client.GetStringAsync("https://api.ipify.org");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to retrieve external IP Address: [{ex}]");
                    return -1;
                }

                if (string.IsNullOrWhiteSpace(currentIpAddress))
                {
                    Console.WriteLine($"Unable to retrieve external IP Address.");
                    return -1;
                }

                string previousIpAddress = null;
                string fileName = @"/ipmon/ip-address.txt";
                try
                {
                    previousIpAddress = File.ReadAllText(fileName);
                }
                catch { }


                if ( !string.IsNullOrWhiteSpace(previousIpAddress)
                    && previousIpAddress.Equals(currentIpAddress, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"IP Address remains unchanged from [{currentIpAddress}]");

                    // IP Address unchanged, nothing to do, exit
                    return 0;
                }

                FileInfo file = new FileInfo(fileName);
                file.Directory.Create(); // If the directory already exists, this method does nothing.

                Console.WriteLine($"IP Address changed from [{previousIpAddress}] to [{currentIpAddress}]");
                File.WriteAllText(fileName, currentIpAddress);

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            
            return 0;
        }
    }
}
