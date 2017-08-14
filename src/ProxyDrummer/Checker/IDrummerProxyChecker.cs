using ProxyDrummer.Proxy;

namespace ProxyDrummer.Checker
{
    public interface IDrummerProxyChecker
    {
        IDrummerProxy GetIfAlive();
    }
}