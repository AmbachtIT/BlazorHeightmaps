using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CaseExtensions;
using Ambacht.Common.Serialization;

namespace Ambacht.Common.Repository
{
    public class OnDiskRepository<T> : IRepository<T>
    {

        public OnDiskRepository(IJsonSerializer serializer, string path)
        {
            this.serializer = serializer;
            this.path = Path.Combine(path, typeof(T).Name.ToSnakeCase());
        }

        private readonly IJsonSerializer serializer;
        private readonly string path;

        private string GetPath(string key)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return Path.Combine(path, $"{key}.json");
        }


        public Task Init(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public async Task Store(string key, T item, CancellationToken token)
        {
            var json = serializer.SerializeObject(item);
            await File.WriteAllTextAsync(GetPath(key), json, token);
        }

        public Task Delete(string key, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<T> Get(string key, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Query()
        {
            return QueryEnumerable().AsQueryable();
        }

        public IEnumerable<T> QueryEnumerable()
        {
            foreach (var file in Directory.GetFiles(path, "*.json"))
            {
                var json = File.ReadAllText(file);
                var result = serializer.DeserializeObject<T>(json);
                yield return result;
            }
        }


        public Task BulkUpsert(List<T> items, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
