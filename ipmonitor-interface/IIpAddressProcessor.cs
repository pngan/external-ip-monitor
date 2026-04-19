using System.Threading.Tasks;

namespace ipmonitor_interface
{
    public interface IIpAddressProcessor
    {
        /// <summary>
        /// Updates all managed DNS A Records to <paramref name="newIpAddress"/>
        /// </summary>
        Task ProcessNewIpAddress(string newIpAddress);
    }
}