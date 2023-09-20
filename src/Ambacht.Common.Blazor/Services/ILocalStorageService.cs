namespace Ambacht.Common.Blazor.Services;

public interface ILocalStorageService
{

    ValueTask<T> GetAsync<T>(string key);

    ValueTask<object> SetAsync(string key, object value);

    ValueTask<object> DeleteAsync(string key);

}