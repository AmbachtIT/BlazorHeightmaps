using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Services
{
    public interface IIdGenerator
    {

        string GenerateId(int byteLength);

    }
}
