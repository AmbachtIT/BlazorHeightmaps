using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Ambacht.Common.Configuration
{
    public static class ConfigExtensions
    {

        public static string GetRequiredValue(this IConfiguration configuration, string key)
        {
            var result = configuration[key];
            if (string.IsNullOrEmpty(result))
            {
                throw new InvalidOperationException($"Could not find configuration value for key {key}");
            }

            return result;
        }

    }
}
