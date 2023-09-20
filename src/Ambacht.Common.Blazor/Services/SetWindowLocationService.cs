using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Ambacht.Common.Blazor.Services
{
    public class SetWindowLocationService : ISetWindowLocationService
    {

        public SetWindowLocationService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        private readonly IJSRuntime _jsRuntime;

        public async Task SetWindowLocation(string address)
        {
            await _jsRuntime.InvokeVoidAsync("Ambacht.setWindowLocation", address);
        }
    }
}
