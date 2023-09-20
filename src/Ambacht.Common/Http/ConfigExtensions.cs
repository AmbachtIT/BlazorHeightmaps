using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ambacht.Common.Http
{
    public static class ConfigExtensions
    {

        public static string GetBaseAddress(this IConfiguration configuration, string key)
        {
            var value = configuration[key];
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"Base address {key} has not been configured");
            }

            if (!value.EndsWith("/"))
            {
                throw new InvalidOperationException($"Base address {key} should end with a trailing slash");
            }

            return value;
        }


        public static IHttpClientBuilder ConfigureProxy(this IHttpClientBuilder builder, string key)
        {
            builder.ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var options = new ProxyOptions();
                config.GetSection(key).Bind(options);

                var result = new HttpClientHandler();
                if (options.IsEnabled && !string.IsNullOrEmpty(options.Hostname) && !string.IsNullOrEmpty(options.Password) && !string.IsNullOrEmpty(options.Username))
                {
                    result.Proxy = new Proxy(options);
                }

                return result;
            });
            return builder;
        }

    }
}