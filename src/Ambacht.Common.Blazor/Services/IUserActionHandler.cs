using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Blazor.Services
{
    public interface IUserActionHandler
    {

        Task Do(Func<Task>  action, string title = "Er is een fout opgetreden.", string message = "Er is een fout opgetreden. De fout is gelogd.");

    }
}
