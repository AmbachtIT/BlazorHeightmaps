using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.IO
{
    public static class PathHelper
    {

        public static string GetFilename(string path)
        {
            var parts = path.Split('/');
            var result = parts.Last();
            if (string.IsNullOrEmpty(result))
            {
                throw new ArgumentException($"Path {parts} does not point to a file.");
            }

            return result;
        }

        /// <summary>
        /// Gets the name of the directory, including trailing slash
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetDirectory(string path)
        {
            if (path.EndsWith('/'))
            {
                return path;
            }
            var parts = path.Split('/');
            return string.Join("/", parts.Take(parts.Length - 1)) + "/";
        }

        /// <summary>
        /// Returns extension, including the leading dot
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string GetExtension(string path)
        {
            if (path.EndsWith('/'))
            {
                throw new ArgumentException("Paths do not have an extension");
            }
            var filename = GetFilename(path);
            return "." + filename.Split('.').Last();
        }

        /// <summary>
        /// Validates that the path is a directory
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void ValidateDirectory(string path)
        {
            if (!path.EndsWith("/"))
            {
                throw new ArgumentException($"{path} is not a directory. All directory paths should end with a forward slash '/'");
            }
        }
    }
}
