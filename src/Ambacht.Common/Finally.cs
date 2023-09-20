using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common
{
    public class Finally : IDisposable
    {
        public Finally(Action finallyAction)
        {
            _finallyAction = finallyAction;
        }

        private readonly Action _finallyAction;

        public void Dispose()
        {
            _finallyAction.Invoke();
        }
    }
}
