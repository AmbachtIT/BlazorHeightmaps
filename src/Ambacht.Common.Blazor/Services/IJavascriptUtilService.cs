using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;

namespace Ambacht.Common.Blazor.Services
{
    public interface IJavascriptUtilService
    {
        Task ScrollToTop();

        Task<Rectangle<float>> GetBounds(ElementReference element);
    }
}
