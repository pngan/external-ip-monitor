using System.Threading.Tasks;

namespace ipmonitor_interface
{
    public interface IIpMonitor
    {
        Task<int> RunIpMonitor();
    }
}