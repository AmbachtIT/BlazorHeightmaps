using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Ambacht.Common.UX
{
    public class Result
    {

        public bool IsSuccess { get; set; }

        public string Message { get; set; }

    }

    public class Result<T> : Result
    {
        public T Payload { get; set; }

        public Result<T2> Convert<T2>(Func<T, T2> convert)
        {
            var result = new Result<T2>()
            {
                IsSuccess = IsSuccess,
                Message = Message
            };
            if (IsSuccess)
            {
                result.Payload = convert(Payload);
            }
            return result;
        }

        public void ThrowIfError()
        {
            if (!IsSuccess)
            {
                throw new InvalidOperationException(Message);
            }

        }
    }
}
