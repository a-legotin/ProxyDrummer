using System.Collections.Generic;
using System.Text.RegularExpressions;
using ProxyDrummer.Proxy;

namespace ProxyDrummer.Parser
{
    public class ProxyParser
    {
        private readonly string[] source;

        public ProxyParser(string[] source)
        {
            this.source = source;
        }

        private const string IpAddressRegex = @"((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?):\d+";
        public List<IDrummerProxy> Parse()
        {
            var proxies = new List<IDrummerProxy>();
            foreach (var textLine in source)
            {
                var cleanText = textLine.Trim().ToLowerInvariant();
                cleanText = Regex.Replace(cleanText, @"\s+", ":");

                var matches = Regex.Matches(cleanText, IpAddressRegex, RegexOptions.IgnoreCase);

                foreach (Match ipAddress in matches)
                {
                    var proxyParts = ipAddress.Value.Split(':');
                    if (proxyParts.Length < 2) continue;
                    var drummerProxy = new DrummerProxy(proxyParts[0], int.Parse(proxyParts[1]));
                    proxies.Add(drummerProxy);
                }
            }
            return proxies;
        }
    }
}
