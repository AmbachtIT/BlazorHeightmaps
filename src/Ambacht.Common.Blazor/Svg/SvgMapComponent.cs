using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;
using Microsoft.AspNetCore.Components;

namespace Ambacht.Common.Blazor.Svg
{
    public class SvgMapComponent : ComponentBase
    {

        [CascadingParameter()]
        public WorldView<float> View { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            View.PropertyChanged += ViewOnPropertyChanged;
        }

        private void ViewOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            StateHasChanged();
        }
    }
}
