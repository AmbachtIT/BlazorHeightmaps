using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Services
{
    public interface INowService
    {

        DateTime Now { get; }

        DateTime UtcNow { get; }

    }

    public class NowService : INowService
    {
        public DateTime Now => DateTime.Now;

        public DateTime UtcNow => DateTime.UtcNow;
    }
}
