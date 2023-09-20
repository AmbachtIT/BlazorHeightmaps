using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor;

namespace Ambacht.Common.Blazor.Services
{
    internal class ToastService : IToastService
    {

        public ToastService(ISnackbar snackbar)
        {
            _snackbar = snackbar;
        }

        private readonly ISnackbar _snackbar;


        public void Success(string title, string message)
        {
            _snackbar.Add(message, Severity.Success);
        }

        public void Info(string title, string message)
        {
            _snackbar.Add(message, Severity.Info);
        }

        public void Warning(string title, string message)
        {
            _snackbar.Add(message, Severity.Warning);
        }

        public void Error(string title, string message)
        {
            _snackbar.Add(message, Severity.Error);
        }
    }
}
