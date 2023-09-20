using System.Text.Json;
using System.Text.Json.Serialization;
using Ambacht.Common.Serialization;

namespace Ambacht.Common.Repository
{
    public class InMemoryRepository<T> : IRepository<T>
    {
        public InMemoryRepository(IJsonSerializer serializer, IEnumerable<T> seedItems, Func<T, string> getId)
        {
            this.serializer = serializer;
            this._getId = getId;
            if (seedItems != null)
            {
                foreach (var item in seedItems)
                {
                    _items.Add(getId(item), item);
                }
            }
        }

        private readonly Func<T, string> _getId;

        public Task Init(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public Task Store(string key, T item, CancellationToken token)
        {
            _items[key] = serializer.Clone(item);
            return Task.CompletedTask;
        }

        public Task Delete(string key, CancellationToken token)
        {
            _items.Remove(key);
            return Task.CompletedTask;
        }

        public Task<T> Get(string key, CancellationToken token)
        {
            if (_items.TryGetValue(key, out var result))
            {
                return Task.FromResult(serializer.Clone(result));
            }

            return Task.FromResult(default(T));
        }

        public IQueryable<T> Query()
        {
            return QueryEnum().AsQueryable();
        }

        private IEnumerable<T> QueryEnum()
        {
            foreach (var item in _items.Values.ToList())
            {
                yield return serializer.Clone(item);
            }
        }

        public async Task BulkUpsert(List<T> items, CancellationToken token)
        {
            foreach (var item in items)
            {
                await Store(_getId(item), item, token);
            }
        }


        private readonly IJsonSerializer serializer;
        private readonly Dictionary<string, T> _items = new Dictionary<string, T>();


    }
}