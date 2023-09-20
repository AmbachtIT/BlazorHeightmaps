using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Diagnostics
{
    /// <summary>
    /// Decorate an assembly with this attribute to indicate that it should not be used from a frontend
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class NoFrontendAttribute : Attribute
    {
    }
}
