using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Ambacht.Common
{
    public static class DIExtensions
    {

        /// <summary>
        /// Creates a service, using the provided overridden services instead of the ones provided by the service provider
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="providedServices"></param>
        /// <returns></returns>
        public static T GetServiceWithOverride<T>(this IServiceProvider provider, Dictionary<string, object> overrideServices)
        {
            var type = typeof(T);
            var constructor = type.GetConstructors().Single(); // This only works with a single constructor
            return provider.GetServiceWithOverride<T>(constructor, overrideServices);
        }

        /// <summary>
        /// Creates a service, using the provided overridden services instead of the ones provided by the service provider
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="providedServices"></param>
        /// <returns></returns>
        private static T GetServiceWithOverride<T>(this IServiceProvider provider, ConstructorInfo constructor, Dictionary<string, object> overrideServices)
        {
            var parameters = constructor.GetParameters();
            var arguments = new object[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                if (overrideServices.TryGetValue(parameters[i].Name, out var arg))
                {
                    arguments[i] = arg;
                }
                else
                {
                    arguments[i] = provider.GetService(parameters[i].ParameterType);
                }
            }
            return (T) constructor.Invoke(arguments);
        }

    }
}
