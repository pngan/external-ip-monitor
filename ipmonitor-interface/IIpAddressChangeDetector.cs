using System.Threading.Tasks;

namespace ipmonitor_interface
{
    public interface IIpAddressChangeDetector
    {
        Task<IIpAddressChangedResult> HasIpAddressChanged();
    }
}