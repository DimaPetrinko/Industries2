using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Time.StepTime.Implementation
{
	internal class ScheduledAction
	{
		private readonly TaskCompletionSource<bool> mTcs;
		private readonly double mScheduledTime;

		public Task Task => mTcs.Task;

		public ScheduledAction(
			double scheduledTime,
			Action<ScheduledAction> onResulted,
			CancellationToken token
		)
		{
			mTcs = new TaskCompletionSource<bool>();
			token.Register(() =>
			{
				onResulted(this);
				mTcs.TrySetCanceled();
			});
			mScheduledTime = scheduledTime;
		}

		public bool CanBeTriggered(double currentTime)
		{
			return currentTime >= mScheduledTime;
		}

		public void Trigger()
		{
			mTcs.TrySetResult(true);
		}
	}
}