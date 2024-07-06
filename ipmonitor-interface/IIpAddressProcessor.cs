using System.Threading.Tasks;

namespace ipmonitor_interface
{
    public interface IIpAddressProcessor
    {
        /// <summary>
        /// Sets the <paramref name="newIpAddress"/> for the DNS A Record <paramref name="dnsARecord"/>
        /// </summary>
        /// <param name="dnsARecord"></param>
        /// <param name="newIpAddress"></param>
        /// <returns></returns>
        Task ProcessNewIpAddress(string dnsARecord, string newIpAddress);
    }
}