using ipmonitor_interface;

namespace ipmonitor_model
{
    public class IpAddressChangedResult : IIpAddressChangedResult
    {
        public IpAddressChangedResult() : this(false, string.Empty, string.Empty)
        {
        }

        public IpAddressChangedResult(bool ipAddressHasChanged, string oldIpAddress, string newIpAddress)
        {
            IpAddressHasChanged = ipAddressHasChanged;
            OldIpAddress = oldIpAddress;
            NewIpAddress = newIpAddress;
        }

        public bool IpAddressHasChanged { get; }
        public string OldIpAddress { get; }
        public string NewIpAddress { get; }
    }
}
