using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyDrummer.Http
{
    public class RotatingHandler : DelegatingHandler
    {
        private List<IWebProxy> _proxies;
        private int _requestsLimitPerProxy = int.MaxValue;
        private int _currentRequestsCount = 0;
        public RotatingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }

        public RotatingHandler UseProxies(List<IWebProxy> proxies)
        {
            _proxies = proxies;
            return this;
        }

        public RotatingHandler SetRequestsLimit(int requestsLimitPerProxy)
        {
            _requestsLimitPerProxy = requestsLimitPerProxy;
            return this;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var random = new Random(DateTime.Now.Millisecond);
            if (_currentRequestsCount > _requestsLimitPerProxy)
            {
                var proxyToUse = _proxies[random.Next(0, _proxies.Count - 1)];
                InnerHandler = new HttpClientHandler() { Proxy = proxyToUse };
                _currentRequestsCount = 0;
            }
            var response = await base.SendAsync(request, cancellationToken);
            _currentRequestsCount++;
            return response;
        }
    }

    public class RetryingHandler : DelegatingHandler
    {
        private List<IWebProxy> _proxies;
        private int _maxRetries = 1;
        public RetryingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }

        public RetryingHandler UseProxies(List<IWebProxy> proxies)
        {
            _proxies = proxies;
            return this;
        }

        public RetryingHandler SetMaximumRetryCount(List<IWebProxy> proxies)
        {
            _proxies = proxies;
            return this;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            var response = new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            for (var i = 0; i < _maxRetries; i++)
            {
                response = await base.SendAsync(request, cancellationToken);
                var proxyToUse = _proxies.FirstOrDefault();
                InnerHandler = new HttpClientHandler() { Proxy = proxyToUse };
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                _proxies?.Remove(proxyToUse);
            }

            return response;
        }
    }
}
