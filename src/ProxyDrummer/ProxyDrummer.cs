using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProxyDrummer.Checker;
using ProxyDrummer.Grabber;
using ProxyDrummer.Proxy;

namespace ProxyDrummer
{
    public class ProxyDrummer
    {
        private string[] _proxySources;

        private IList<IDrummerProxy> _proxies;
        private IGrabbersFactory _grabbersFactory;

        public ProxyDrummer FromWeb(params string[] proxySources)
        {
            _proxySources = proxySources;
            _grabbersFactory = new WebGrabbersFactory();
            return this;
        }

        public ProxyDrummer FromFiles(params string[] proxyFiles)
        {
            _proxySources = proxyFiles;
            _grabbersFactory = new FileGrabbersFactory();
            return this;
        }

        public async Task<IList<IDrummerProxy>> GetAliveAsync()
        {
            var aliveProxies = new List<IDrummerProxy>();
            foreach (var proxySource in _proxySources)
            {
                var grabber = _grabbersFactory.GetGrabber(proxySource);
                _proxies = await grabber.GetProxiesAsync();
                aliveProxies.AddRange(_proxies.Select(drummerProxy => new DrummerProxyChecker(drummerProxy)).Select(checker => checker.GetIfAlive()).Where(checkedPr => checkedPr != null));
            }

            return aliveProxies;
        }

        public async Task<IList<IDrummerProxy>> ParseAsync()
        {
            var proxies = new List<IDrummerProxy>();
            foreach (var proxySource in _proxySources)
            {
                var grabber = _grabbersFactory.GetGrabber(proxySource);
                var parsed = await grabber.GetProxiesAsync();
                proxies.AddRange(parsed);
            }
            return proxies;
        }

    }
}
