using ProxyDrummer.Classes;

namespace ProxyDrummer.Proxy
{
    public interface IDrummerProxy
    {
        string Address { get; }
        int Port { get; }
        IPStatus Status { get; set; }
    }
}
