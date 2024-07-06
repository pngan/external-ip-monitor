using ipmonitor_model;

namespace ipmonitor
{
    public static class IpMonitor
    {
        public static IpAddressChangedResult HasIpAddressChanged()
        {
            return new IpAddressChangedResult();
        }
    }
}