using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Ambacht.Common.Http
{
    public interface IHostService
    {

        /// <summary>
        /// Returns the host, this app is running on, without the trailing slash
        /// </summary>
        /// <returns></returns>
        string GetHost();

    }

    public class HostService : IHostService
    {

        public HostService(IConfiguration config)
        {
            var host = config["Urls:Api"];
            if (string.IsNullOrEmpty(host))
            {
                throw new InvalidOperationException("Urls:Api should be configured in appsettings.json");
            }
            if (host.EndsWith("/"))
            {
                host = host.Substring(0, host.Length - 1);
            }
            _host = host;
        }

        private readonly string _host;

        public string GetHost()
        {
            return _host;
        }
    }
}
