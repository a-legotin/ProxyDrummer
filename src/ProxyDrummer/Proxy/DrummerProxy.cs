using System;
using System.Net;
using System.Net.NetworkInformation;
using ProxyDrummer.Classes;

namespace ProxyDrummer.Proxy
{
    public class DrummerProxy : IWebProxy, IDrummerProxy
    {
        public ICredentials Credentials { get; set; }
        public Uri GetProxy(Uri destination)
        {
            return new Uri($"http://{Address}:{Port}");
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }

        public DrummerProxy(string address, int port)
        {
            Address = address;
            Port = port;
            Status = IPStatus.Unknown;
        }

        public string Address { get; }
        public int Port { get; }
        public IPStatus Status { get; set; }
    }
}
