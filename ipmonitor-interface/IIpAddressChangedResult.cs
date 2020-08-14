namespace ipmonitor_interface
{
    public interface IIpAddressChangedResult
    {
        bool IpAddressHasChanged { get; }
        string OldIpAddress { get; }
        string NewIpAddress { get; }
    }
}