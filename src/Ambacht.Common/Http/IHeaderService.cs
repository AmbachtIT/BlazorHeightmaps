using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Http
{
    public interface IHeaderService
    {
        IEnumerable<KeyValuePair<string, string>> Headers();

        void SetHeader(string name, string value);

        void ClearHeader(string name);
    }
}
