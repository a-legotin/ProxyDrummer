using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyDrummer.Grabber
{
    public interface IGrabbersFactory
    {
        IProxyGrabber GetGrabber(string source);
    }
    public class WebGrabbersFactory : IGrabbersFactory
    {
        public IProxyGrabber GetGrabber(string source)
        {
            return new WebProxyGrabber(source);
        }
    }

    public class FileGrabbersFactory : IGrabbersFactory
    {
        public IProxyGrabber GetGrabber(string source)
        {
            return new FileProxyGrabber(source);
        }
    }
}
