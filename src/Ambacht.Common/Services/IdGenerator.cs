using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Services
{
    public class IdGenerator : IIdGenerator
    {
        public string GenerateId(int byteLength)
        {
            var bytes = RandomNumberGenerator.GetBytes(byteLength);
            var result = Convert.ToBase64String(bytes);
            var desiredStringLength = result.Length;
            do
            {
                var chars =
                    (result + Convert.ToBase64String(RandomNumberGenerator.GetBytes(byteLength)))
                    .Where(char.IsLetterOrDigit)
                    .ToArray();
                result = new string(chars)
                    .Substring(0, desiredStringLength);
            } while (result.Length < desiredStringLength);

            return result;
        }
    }
}
