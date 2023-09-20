using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Ambacht.Common.Services
{

    /// <summary>
    /// Indicates that this service needs to be initialized on application startup. The operation blocks application startup, so please don't do anything that takes too long here
    /// </summary>
    public interface IAsyncInit
    {

        Task InitAsync(CancellationToken token);

    }

    public static class IAsyncInitServiceProviderExtensions
    {

        public static async Task InitAsync(this IServiceProvider serviceProvider, CancellationToken token = default)
        {
            foreach (var item in serviceProvider.GetServices<IAsyncInit>())
            {
                await item.InitAsync(token);
            }
        }

    }

}
