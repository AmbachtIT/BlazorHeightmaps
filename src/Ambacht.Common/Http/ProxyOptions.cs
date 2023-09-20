using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Http
{
    public class ProxyOptions
    {


        public bool IsEnabled { get; set; } = true;

        public string Username { get; set; }

        public string Password { get; set; }

        public string Hostname { get; set; }

    }
}
