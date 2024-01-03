using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;

namespace Core.Time.StepTime.Implementation
{
	internal class StepTimeModel : IStepTimeModel
	{
		private readonly ITicker mTicker;
		private readonly float mDeltaTime;
		private CancellationTokenSource mCts;

		public StepTimeModel(ITicker ticker, float tickInterval)
		{
			mTicker = ticker;
			mDeltaTime = tickInterval;
		}

		public void StartTicking()
		{
			StartTickingInternal().Forget(StopTicking);
		}

		private async Task StartTickingInternal()
		{
			mCts = new CancellationTokenSource();
			while (!mCts.Token.IsCancellationRequested)
			{
				await Task.Delay((int)(mDeltaTime * 1000f));
				mTicker.Tick(mDeltaTime);
			}
		}

		public void StopTicking()
		{
			if (mCts != null && !mCts.IsCancellationRequested) mCts.Cancel();
		}
	}
}