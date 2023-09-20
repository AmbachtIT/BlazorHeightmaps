using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.UX
{
	public class Debounce
	{

		private readonly TimeSpan delay;
		private CancellationTokenSource cancellationTokenSource;
		private DateTime lastInvocation;

		public Debounce(TimeSpan delay)
		{
			this.delay = delay;
		}

		public async Task Invoke(Func<CancellationToken, Task> task)
		{
			cancellationTokenSource?.Cancel();
			cancellationTokenSource = null;

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
