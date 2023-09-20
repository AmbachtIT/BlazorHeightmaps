using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Http
{
    public class ApiRequestOptions
    {

        public string AccessToken { get; set; }

        public void Populate(HttpRequestHeaders headers)
        {
            if (!string.IsNullOrEmpty(AccessToken))
            {
                headers.Add("authorization", $"Bearer {AccessToken}");
            }
        }
    }
}
