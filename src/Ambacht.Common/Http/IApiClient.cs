using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Serialization;
using Ambacht.Common.Services;


namespace Ambacht.Common.Http
{
    public interface IApiClient
    {
        Task<T> PostJson<T>(string url, object payload, ApiRequestOptions options);

        Task<T> GetJson<T>(string url, ApiRequestOptions options);
    }

    public static class ApiClientExtensions
    {
        public static FactoryBuilder<IApiClient> AddApiClientFactory(this IServiceCollection services)
        {
            services.AddFactory<IApiClient>();
            return new FactoryBuilder<IApiClient>(services);
        }

        public static FactoryBuilder<IApiClient> ConfigureApiClients(this IServiceCollection services)
        {
            return new FactoryBuilder<IApiClient>(services);
        }

        public static void AddApiClient(this FactoryBuilder<IApiClient> builder, string key)
        {
            builder.AddImplementation(key,
                sp =>
                {
                    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                    var serializer = sp.CreateFactory<string, IJsonSerializer>(key)();
                    return new ApiClient(() =>
                            httpClientFactory.CreateClient(key),
                        serializer);
                });
        }

        public static void AddApiClient(this FactoryBuilder<IApiClient> builder)
        {
            builder.AddImplementation(sp =>
                {
                    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                    var serializer = sp.CreateFactory<string, IJsonSerializer>("Default")();
                    return new ApiClient(() =>
                            httpClientFactory.CreateClient("Default"),
                        serializer);
                });
        }


        public static Task<T> PostJson<T>(this IApiClient client, string url, object payload)
        {
            return client.PostJson<T>(url, payload, new ApiRequestOptions());
        }

        public static Task<T> GetJson<T>(this IApiClient client, string url)
        {
            return client.GetJson<T>(url, new ApiRequestOptions());
        }
    }
}
