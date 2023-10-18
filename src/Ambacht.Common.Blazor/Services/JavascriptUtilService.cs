using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;

namespace Ambacht.Common.Blazor.Services
{
    public class JavascriptUtilService : IJavascriptUtilService
    {

        public JavascriptUtilService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        private readonly IJSRuntime _jsRuntime;

        public async Task ScrollToTop() => await _jsRuntime.InvokeVoidAsync("Ambacht.scrollToTop");


        public async Task<Rectangle<float>> GetBounds(ElementReference element)
        {
            return await _jsRuntime.InvokeAsync<Rectangle<float>>("Ambacht.getBounds", element);
        }


    }
}
