using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.IO
{
    public class LocalFileSystem : IFileSystem
    {

        public LocalFileSystem(string basePath)
        {
            _basePath = basePath;
        }

        private readonly string _basePath;

        private string GetLocalPath(string path) => Path.Combine(_basePath, path);


        public Task<Stream> OpenRead(string path, CancellationToken token = default)
        {
            return Task.FromResult<Stream>(File.OpenRead(GetLocalPath(path)));
        }

        public Task<Stream> OpenWrite(string path, CancellationToken token = default)
        {
            var local = GetLocalPath(path);
            var dir = Path.GetDirectoryName(local);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return Task.FromResult<Stream>(File.Create(local));
        }

        public Task<bool> Exists(string path, CancellationToken token = default)
        {
            return Task.FromResult(File.Exists(GetLocalPath(path)));
        }


        public override string ToString() => $"Files @ {_basePath}";
    }
}
