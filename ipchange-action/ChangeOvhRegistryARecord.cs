using System;
using System.Threading.Tasks;
using ipmonitor_interface;

namespace ipchange_action
{
    public class ChangeOvhRegistryARecord : IIpAddressProcessor
    {
        public async Task ProcessNewIpAddress(string newIpAddress)
        {
            Console.WriteLine($"Processing new IP Address {newIpAddress}");
            await Task.CompletedTask;
        }
    }
}