using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Diagnostics
{
    public class Progress : INotifyPropertyChanged
    {
        public Progress(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }

        public CancellationToken CancellationToken { get; }

        public string CurrentTask { get; set; }

        public string FormattedProgress { get; set; }

        public double? ProgressPercentage { get; set; }


        public void Report(string currentTask, double? progressPercentage = null, string formattedProgress = null)
        {
            this.CurrentTask = currentTask;
            this.ProgressPercentage = progressPercentage;
            this.FormattedProgress = formattedProgress ?? FormatProgress(progressPercentage);
            OnPropertyChanged(nameof(CurrentTask));
            OnPropertyChanged(nameof(ProgressPercentage));
            OnPropertyChanged(nameof(FormattedProgress));
        }

        private string FormatProgress(double? progress)
        {
            return progress.HasValue ? $"{progress.Value:0}%" : "working...";
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly Progress Empty = new Progress(new CancellationToken());

    }
}
