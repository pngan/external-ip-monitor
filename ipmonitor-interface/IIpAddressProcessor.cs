using System.Threading.Tasks;

namespace ipmonitor_interface
{
    public interface IIpAddressProcessor
    {
        Task ProcessNewIpAddress(string newIpAddress);
    }
}