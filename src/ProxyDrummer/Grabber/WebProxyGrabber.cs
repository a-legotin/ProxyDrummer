using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProxyDrummer.Parser;
using ProxyDrummer.Proxy;

namespace ProxyDrummer.Grabber
{
    public interface IProxyGrabber
    {
        Task<IList<IDrummerProxy>> GetProxiesAsync();
    }

    public class FileProxyGrabber : IProxyGrabber
    {
        private readonly string filePath;
        private const string proxyPattern = @"\d{1,3}(\.\d{1,3}){3}:\d{1,5}";

        public FileProxyGrabber(string filePath)
        {
            this.filePath = filePath;
        }
        public async Task<IList<IDrummerProxy>> GetProxiesAsync()
        {
            var proxies = new List<IDrummerProxy>();
            if (!File.Exists(filePath)) return proxies;

            var lines = File.ReadAllLines(filePath);
            if (lines == null || lines.Length < 1) return proxies;
            await Task.Run(() =>
            {
                var parser = new ProxyParser(lines);
                proxies = parser.Parse();
            });
            return proxies;
        }
    }

    public class WebProxyGrabber : IProxyGrabber
    {
        private readonly string proxySourceUrl;
        private const string proxyPattern = @"\d{1,3}(\.\d{1,3}){3}:\d{1,5}";
        public WebProxyGrabber(string proxySourceUrl)
        {
            this.proxySourceUrl = proxySourceUrl;
        }

        public async Task<IList<IDrummerProxy>> GetProxiesAsync()
        {
            var proxies = new List<IDrummerProxy>();
            try
            {
                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync(new Uri(proxySourceUrl)))
                    {
                        using (var content = response.Content)
                        {
                            var htmlString = await content.ReadAsStringAsync();
                            var matches = Regex.Matches(htmlString, proxyPattern);
                            foreach (Match match in matches)
                            {
                                if (!match.Success) continue;
                                var proxy = match.Value;
                                var proxyParts = proxy.Split(':');
                                var drummerProxy = new DrummerProxy(proxyParts[0], int.Parse(proxyParts[1]));
                                proxies.Add(drummerProxy);
                            }

                        }
                    }
                }
            }
            catch
            {

            }

            return proxies;
        }
    }
}
