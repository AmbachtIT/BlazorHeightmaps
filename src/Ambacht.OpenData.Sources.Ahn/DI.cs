using Ambacht.Common.IO;
using Microsoft.Extensions.DependencyInjection;

namespace Ambacht.OpenData.Sources.Ahn
{
    public static class DI
    {

        public static IServiceCollection AddAhn(this IServiceCollection services, Func<IServiceProvider, IFileSystem> getFileSystem)
        {
            services.AddTransient<AhnPipeline>(sp => new AhnPipeline(getFileSystem(sp), sp.GetRequiredService<IHttpClientFactory>()));
            return services;
        }

    }
}