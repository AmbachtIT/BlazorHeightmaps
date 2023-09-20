using Microsoft.JSInterop;

namespace Ambacht.Common.Blazor.Services;

public class LocalStorageService : ILocalStorageService
{

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    private readonly IJSRuntime _jsRuntime;

    public ValueTask<T> GetAsync<T>(string key)
        => _jsRuntime.InvokeAsync<T>("blazorLocalStorage.get", key);

    public ValueTask<object> SetAsync(string key, object value)
        => _jsRuntime.InvokeAsync<object>("blazorLocalStorage.set", key, value);

    public ValueTask<object> DeleteAsync(string key)
        => _jsRuntime.InvokeAsync<object>("blazorLocalStorage.delete", key);
    
    
}
