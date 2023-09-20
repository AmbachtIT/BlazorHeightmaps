using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common
{

    /// <summary>
    /// Encapsulates a string identifier. The type parameter TValue is used to define the scope of the identifier.
    ///
    /// This makes it possible to use the compiler to prevent assigning the wrong type of id.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="Id"></param>
    public record struct Identifier<TValue>(string Id)
    {
        public override string ToString() => Id;

        public string ToLower() => Id?.ToLower();

    }
}
