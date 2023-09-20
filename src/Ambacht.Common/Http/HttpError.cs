using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Http
{
    public record class HttpError(HttpStatusCode Code, string Message)
    {

        public HttpError(HttpStatusCode code) : this(code, Enum.GetName(typeof(HttpStatusCode), code))
        {
        }

        public override string ToString() => $"HTTP/{Code} {Message}";

        public static HttpError FromResponse(HttpResponseMessage response) => new(response.StatusCode, response.ReasonPhrase);
    }
}
