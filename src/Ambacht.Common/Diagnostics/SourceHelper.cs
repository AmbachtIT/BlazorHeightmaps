using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Diagnostics
{
    public static class SourceHelper
    {

        public static string GetSourceRoot(params string[] paths)
        {
            var path = new FileInfo(typeof(SourceHelper).Assembly.Location).Directory;
            while (Directory.GetFiles(path.FullName, "*.sln").Length == 0)
            {
                path = path.Parent;
            }

            var result = Path.Combine(new[]
            {
                path.FullName
            }.Concat(paths).ToArray());
            return result;
        }

    }
}
