using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using System.Reflection.Metadata;
using Ambacht.Common.Http;
using Ambacht.Common.Serialization;


namespace Ambacht.Common.Http
{
    public class ApiClient : IApiClient
    {

        public ApiClient(Func<HttpClient> httpClientFactory, IJsonSerializer serializer)
        {
            _httpClientFactory = httpClientFactory;
            _serializer = serializer;
        }

        private readonly Func<HttpClient> _httpClientFactory;
        private readonly IJsonSerializer _serializer;

        public async Task<T> PostJson<T>(string url, object payload, ApiRequestOptions options)
        {
            using var client = _httpClientFactory();
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = CreateJsonContent(payload);
            options.Populate(request.Headers);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await CreateResponse<T>(response);
        }

        public async Task<T> GetJson<T>(string url, ApiRequestOptions options)
        {
            using var client = _httpClientFactory();
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            options.Populate(request.Headers);
            
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await CreateResponse<T>(response);
        }

        private async Task<T> CreateResponse<T>(HttpResponseMessage response)
        {
            return await ReadJsonContent<T>(response.Content);
        }

        private HttpContent CreateJsonContent(object obj)
        {
            var json = _serializer.SerializeObject(obj);
            return new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        }

        private async Task<T> ReadJsonContent<T>(HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            return _serializer.DeserializeObject<T>(json);
        }


    }
}
