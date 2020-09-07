using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace ErikTheCoder.Sandbox.AsyncPipeline
{
    public class CacheBustingMessageHandler : DelegatingHandler
    {
        public CacheBustingMessageHandler()
        {
            InnerHandler = new HttpClientHandler();
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage Request,
          CancellationToken CancellationToken)
        {
            Request.RequestUri = new Uri($"{Request.RequestUri.AbsoluteUri}&cacheBust={Guid.NewGuid()}");
            return await base.SendAsync(Request, CancellationToken);
        }
    }
}
