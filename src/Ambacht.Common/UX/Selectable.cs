using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.UX
{
    public class Selectable<T>
    {

        public T Value { get; set; }

        public bool IsSelected { get; set; }

    }
}
