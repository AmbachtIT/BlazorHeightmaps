using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common;
using Ambacht.Common.Blazor;
using Ambacht.Common.Diagnostics;
using Ambacht.Common.IO;
using Ambacht.OpenData.Sources.Ahn;
using BlazorHeightmaps.Wasm.App;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorHeightmaps.Test
{
    public static class TestHelper
    {

        public static ServiceProvider CreateServiceProvider(Action<IServiceCollection> configureServices = null)
        {
            var services = new ServiceCollection();
            var config = CreateConfig();
            services.AddTestServices(config, "https://localhost:9999/");
            configureServices?.Invoke(services);
            return services.BuildServiceProvider();
        }

        private static IConfiguration CreateConfig()
        {
            var builder = new ConfigurationBuilder();
            //builder.AddJsonFile("appsettings.json");
            //builder.AddUserSecrets(typeof(TestHelper).Assembly);
            return builder.Build();
        }

        private static void AddTestServices(this IServiceCollection services, IConfiguration config, string baseAddress)
        {
            services.AddSingleton(config);
            services.AddAmbachtCommon();
            services.AddAmbachtCommonBlazor(baseAddress);
            services.AddAhn(sp => new LocalFileSystem(GetSourceRoot(".storage", "ahn")));
            services.AddBlazorHeightmapsWasmApp(baseAddress);
        }


        public static string GetSourceRoot(params string[] paths) => SourceHelper.GetSourceRoot(paths);

    }
}
