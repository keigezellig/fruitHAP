using System;
using System.Threading.Tasks;
using System.Threading;

namespace FruitHAP.Common.Helpers
{
	public static class TaskExtension
	{
		public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout) {

			var timeoutCancellationTokenSource = new CancellationTokenSource();

			var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
			if (completedTask == task) {
				timeoutCancellationTokenSource.Cancel();
				return await task;
			} else {
				throw new TimeoutException("The operation has timed out.");
			}
		}
	}
}

