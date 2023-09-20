using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Ambacht.Common.Blazor.Components
{
    public class Debounce
    {

        private readonly Func<CancellationToken, Task> _task;
        private readonly TimeSpan delay;
        private CancellationTokenSource cancellationTokenSource;
        private DateTime lastInvocation;

        public Debounce(TimeSpan delay, Action task) : this(delay, _ =>
        {
            task();
            return Task.CompletedTask;
        })
        {
        }

        public Debounce(TimeSpan delay, Func<CancellationToken, Task> task = null)
        {
            this.delay = delay;
            this._task = task;
        }

        public Task Invoke() => Invoke(null);

        public async Task Invoke(Func<CancellationToken, Task> task)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = null;

            task = task ?? _task;

            var invoked = DateTime.UtcNow;
            lastInvocation = invoked;
            await Task.Delay(delay);
            if (lastInvocation == invoked)
            {
                cancellationTokenSource = new CancellationTokenSource();
                await task.Invoke(cancellationTokenSource.Token);
            }
        }

    }
}
