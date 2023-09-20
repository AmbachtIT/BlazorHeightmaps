using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Ambacht.Common.Services
{
    public interface IFactory<TKey, TType>
    {

        TType Create(TKey name);

        IEnumerable<TType> All();

    }

    internal class Factory<TKey, TType> : IFactory<TKey, TType>
    {
        public Factory(IEnumerable<FactoryEntry<TKey, TType>> entries)
        {
            _entries = entries.ToArray();
        }

        private readonly FactoryEntry<TKey, TType>[] _entries;


        public TType Create(TKey name)
        {
            return _entries.Single(e => e.Key.Equals(name)).Factory();
        }

        public IEnumerable<TType> All()
        {
            return _entries.Select(e => e.Factory());
        }
    }

    internal class FactoryEntry<TKey, TType>
    {
        public TKey Key { get; init; }
        public Func<TType> Factory { get; init; }
    }


    public static class FactoryExtensions
    {
        public static FactoryBuilder<string, TItem> AddFactory<TItem>(this IServiceCollection services) where TItem : class => services.AddFactory<string, TItem>();


        public static FactoryBuilder<TKey, TItem> AddFactory<TKey, TItem>(this IServiceCollection services) where TItem: class
        {
            services.AddSingleton<IFactory<TKey, TItem>, Factory<TKey, TItem>>();
            return services.ConfigureFactory<TKey, TItem>();
        }

        public static FactoryBuilder<string, TItem> ConfigureFactory<TItem>(this IServiceCollection services) where TItem : class => services.ConfigureFactory<string, TItem>();

        public static FactoryBuilder<TKey, TItem> ConfigureFactory<TKey, TItem>(this IServiceCollection services) where TItem : class
        {
            return new FactoryBuilder<TKey, TItem>(services);
        }


        public static Func<TItem> CreateFactory<TKey, TItem>(this IServiceProvider provider, TKey name)
        {
            var factory = provider.GetRequiredService<IFactory<TKey, TItem>>();
            return () => factory.Create(name);
        }

        public static FactoryBuilder<string, TItem> AddImplementation<TItem>(this FactoryBuilder<string, TItem> builder, Func<IServiceProvider, TItem> factory) where TItem: class
        {
            builder.AddImplementation("Default", factory);
            return builder;
        }

    }

    public class FactoryBuilder<TItem> : FactoryBuilder<string, TItem> where TItem : class
    {
        internal FactoryBuilder(IServiceCollection services) : base(services)
        {
        }
    }

    public class FactoryBuilder<TKey, TItem> where TItem: class
    {

        public FactoryBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        public FactoryBuilder<TKey, TItem> AddImplementation(TKey name, Func<IServiceProvider, TItem> factory)
        {
            if (object.Equals(name, "Default"))
            {
                Services.AddTransient<TItem>(sp => factory(sp));
            }
            Services.AddTransient(sp => new FactoryEntry<TKey, TItem>()
            {
                Key = name,
                Factory = () => factory(sp)
            });
            return this;
        }

        public FactoryBuilder<TKey, TItem> AddImplementation<TImplementation>(TKey name) where TImplementation : class, TItem
        {
            Services.AddTransient<TImplementation>();
            Services.AddTransient(sp => new FactoryEntry<TKey, TItem>()
            {
                Key = name,
                Factory = () => (TItem)sp.GetRequiredService<TImplementation>()
            });
            return this;
        }

    }


}
