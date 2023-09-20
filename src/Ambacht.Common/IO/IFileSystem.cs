using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.IO
{
    public interface IFileSystem
    {

        Task<Stream> OpenRead(string path, CancellationToken token = default);

        Task<Stream> OpenWrite(string path, CancellationToken token = default);

        Task<bool> Exists(string path, CancellationToken token = default);



    }
}
