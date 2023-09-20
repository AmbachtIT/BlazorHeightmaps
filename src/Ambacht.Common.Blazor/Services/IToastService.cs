using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Blazor.Services
{
    public interface IToastService
    {

        void Success(string title, string message);

        void Info(string title, string message);

        void Warning(string title, string message);

        void Error(string title, string message);

    }
}
