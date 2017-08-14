using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProxyDrummer.Classes;
using ProxyDrummer.Proxy;

namespace ProxyDrummer.Checker
{
    public class DrummerProxyChecker : IDrummerProxyChecker
    {
        private readonly IDrummerProxy proxy;

        public DrummerProxyChecker(IDrummerProxy proxy)
        {
            this.proxy = proxy;
        }

        public IDrummerProxy GetIfAlive()
        {
            var pingSucceed = Ping();
            if (!pingSucceed) return null;
            proxy.Status = IPStatus.Alive;
            return proxy;
        }

        private bool Ping()
        {
            try
            {
                var httpHandler = new HttpClientHandler {Proxy = proxy as IWebProxy};
                using (var client = new HttpClient(httpHandler))
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    var resp = client.GetAsync("http://myip.ru/").Result;
                    return resp.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
