using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ambacht.Common.Blazor.Services
{
    public class UserActionHandler : IUserActionHandler
    {

        private readonly IToastService _toastService;
        private readonly ILogger<UserActionHandler> _logger;

        public UserActionHandler(ILogger<UserActionHandler> logger, IToastService toastService)
        {
            _logger = logger;
            _toastService = toastService;
        }

        public async Task Do(Func<Task> action, string title, string message)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{title} - {message}");
                _toastService.Error(title, message);
            }
        }
    }
}
