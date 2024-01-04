using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;

namespace Core.Time.Implementation
{
	internal class StepTimeModel : ITimeModel
	{
		private readonly IEnumerable<ITickable> mTickables;
		private readonly float mDeltaTime;
		private CancellationTokenSource mCts;

		public StepTimeModel(float timeStep, params ITickable[] tickables)
		{
			mTickables = tickables;
			mDeltaTime = timeStep;
		}

		public float Scale { get; set; } = 1f;

		public void Start()
		{
			StartTickingInternal().Forget(Stop);
		}

		private async Task StartTickingInternal()
		{
			mCts = new CancellationTokenSource();
			while (!mCts.Token.IsCancellationRequested)
			{
				var scaledDeltaTime = mDeltaTime / Scale;
				await Task.Delay((int)(scaledDeltaTime * 1000f));
				foreach (var tickable in mTickables)
				{
					tickable.Tick(scaledDeltaTime);
				}
			}
		}

		public void Stop()
		{
			if (mCts != null && !mCts.IsCancellationRequested) mCts.Cancel();
		}
	}
}