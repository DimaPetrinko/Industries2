using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Time.Implementation
{
	internal class ScheduledAction
	{
		private readonly TaskCompletionSource<bool> mTcs;
		private readonly double mStartTime;
		private readonly double mDuration;
		private readonly double mScheduledTime;
		private readonly Action<float> mOnTick;

		public Task Task => mTcs.Task;

		public ScheduledAction(
			double startTime,
			double duration,
			Action<float> onTick,
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
			mStartTime = startTime;
			mDuration = duration;
			mScheduledTime = startTime + duration;
			mOnTick = onTick;
		}

		public bool CanBeTriggered(double currentTime)
		{
			return currentTime >= mScheduledTime;
		}

		public void Tick(double currentTime)
		{
			if (mOnTick == null) return;
			var progress = (float)((currentTime - mStartTime) / mDuration);
			progress = Math.Min(1, Math.Max(0, progress));
			mOnTick(progress);
		}

		public void Trigger()
		{
			mTcs.TrySetResult(true);
		}
	}
}