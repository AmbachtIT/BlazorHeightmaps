using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Http
{
    public class HeaderService : IHeaderService
    {

        private readonly List<KeyValuePair<string, string>> _headers = new List<KeyValuePair<string, string>>();

        public IEnumerable<KeyValuePair<string, string>> Headers()
        {
            foreach (var header in _headers)
            {
                yield return header;
            }
        }

        public void SetHeader(string name, string value)
        {
            ClearHeader(name);
            _headers.Add(new KeyValuePair<string, string>(name, value));
        }

        public void ClearHeader(string name)
        {
            _headers.RemoveAll(kv => kv.Key == name);
        }
    }
}
