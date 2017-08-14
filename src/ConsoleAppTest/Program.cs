using System;
using System.Net;
using System.Net.Http;

namespace ConsoleAppTest
{
    public class Program
    {
        private static int _requestsLimitPerProxy = 5;
        private static int _currentRequestsCount = 0;
        public static void Main(string[] args)
        {
            Console.WriteLine("Checking proxies...");
            var drummer = new ProxyDrummer.ProxyDrummer().FromFiles(@"D:\Dropbox\Soft\dev\proxies\list.txt");
            var _proxies = drummer.GetAliveAsync().Result;
            Console.WriteLine($"Got {_proxies.Count} proxies...");

            //var httpHandler = new RotatingHandler(new HttpClientHandler()).UseProxies(_proxies.Cast<IWebProxy>().ToList()).SetRequestsLimit(5);
            var random = new Random(DateTime.Now.Millisecond);
            var proxyToUse = _proxies[random.Next(0, _proxies.Count - 1)];

            while (true)
            {
                try
                {
                    if (_currentRequestsCount > _requestsLimitPerProxy)
                    {
                        proxyToUse = _proxies[random.Next(0, _proxies.Count - 1)];
                        _currentRequestsCount = 0;
                    }
                    var client = new HttpClient(new HttpClientHandler() { Proxy = proxyToUse as IWebProxy });
                    var request = new HttpRequestMessage(HttpMethod.Get, "http://bot.whatismyipaddress.com/");
                    var response = client.SendAsync(request).Result;
                    _currentRequestsCount++;
                    var html = response.Content.ReadAsStringAsync().Result;
                    response.Dispose();
                    Console.WriteLine(html);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Console.ReadKey();
        }
    }
}
