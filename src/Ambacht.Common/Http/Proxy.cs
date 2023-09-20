using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Http
{
    public class Proxy : IWebProxy
    {

        public Proxy(ProxyOptions config)
        {
            if (!config.IsEnabled)
            {
                throw new InvalidOperationException();
            }
            _username = config.Username;
            _password = config.Password;
            _hostname = config.Hostname;
        }

        private readonly string _username;
        private readonly string _password;
        private readonly string _hostname;

        public ICredentials Credentials
        {
            get { return new NetworkCredential("customer-ambachtit-cc-nl", "bMNxSwQIu2ygy"); }
            set { }
        }

        public Uri GetProxy(Uri destination)
        {
            return new Uri("socks5://pr.oxylabs.io:7777");
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }

    }
}
