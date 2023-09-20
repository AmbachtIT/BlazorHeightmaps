using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ambacht.Common.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Ambacht.Common.Blazor.Components
{
    public class DraggableComponent : ComponentBase
    {

        [CascadingParameter()]
        public DragContainer Container { get; set; }


        [Parameter()]
        public EventCallback<Vector2> Dragged { get; set; }

        [Inject()]
        public IToastService Toast { get; set; }

        public virtual async Task OnDrag(Vector2 v)
        {
            await Dragged.InvokeAsync(v);
        }

        protected void OnMouseDown(MouseEventArgs e)
        {
            Container.OnMouseDown(this, e);
        }

        protected void OnTouchStart(TouchEventArgs e)
        {
            Container.OnTouchStart(this, e);
        }

    }
}
