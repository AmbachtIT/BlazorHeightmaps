using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Ambacht.Common.Blazor.Services
{
    public class DownloadGeneratedFileService : IDownloadGeneratedFileService
    {
        public DownloadGeneratedFileService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        private readonly IJSRuntime _jsRuntime;



        public async Task SaveAs(string filename, byte[] data, string mimeType)
        {
            await _jsRuntime.InvokeAsync<object>("Ambacht.saveAsFile", filename, Convert.ToBase64String(data), mimeType);
        }

    }
}
