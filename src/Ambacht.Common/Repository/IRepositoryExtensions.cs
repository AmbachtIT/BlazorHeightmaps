using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Repository
{
    public static class IRepositoryExtensions
    {

        public static Task Store<TKey, TValue>(this IRepository<TKey, TValue> repository, TKey key, TValue item)
        {
            return repository.Store(key, item, CancellationToken.None);
        }


        public static Task Delete<TKey, TValue>(this IRepository<TKey, TValue> repository, TKey key)
        {
            return repository.Delete(key, CancellationToken.None);
        }

        public static Task<TValue> Get<TKey, TValue>(this IRepository<TKey, TValue> repository, TKey key)
        {
            return repository.Get(key, CancellationToken.None);
        }

        public static Task BulkUpsert<TKey, TValue>(this IRepository<TKey, TValue> repository, List<TValue> items)
        {
            return repository.BulkUpsert(items, CancellationToken.None);
        }


        public static Task AtomicUpdate<TKey, TValue>(this IRepository<TKey, TValue> repository, TKey key,
            Func<TValue, TValue> update,
            CancellationToken token = default, int maxRetries = 5) => repository.AtomicUpdate(key, i => Task.FromResult(update(i)), token, maxRetries);

        public static async Task AtomicUpdate<TKey, TValue>(this IRepository<TKey, TValue> repository, TKey key, Func<TValue, Task<TValue>> update,
            CancellationToken token = default, int maxRetries = 5)
        {
            for (var retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    var item = await repository.Get(key, token);
                    item = await update(item);
                    await repository.Store(key, item, token);
                    return;
                }
                catch (ConcurrentUpdateFailedException)
                {
                    // The update failed. Just try again.
                }
            }

            throw new ConcurrentUpdateFailedException();
        }


    }
}
