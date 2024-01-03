using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Time.StepTime.Implementation
{
	public class StepTimeProvider : ITicker, ITimeProvider
	{
		private readonly List<ScheduledAction> mScheduledActions = new List<ScheduledAction>();
		private readonly List<ScheduledAction> mScheduledActionsToPrune = new List<ScheduledAction>();
		private readonly List<ScheduledAction> mScheduledActionsToTrigger = new List<ScheduledAction>();
		private readonly object mLockObject = new object();

		public double SecondsSinceStart { get; private set; }

		public Task Delay(int milliseconds, CancellationToken token = default)
		{
			var scheduledAction = new ScheduledAction(
				SecondsSinceStart + milliseconds / 1000f,
				sa => mScheduledActionsToPrune.Add(sa),
				token);
			mScheduledActions.Add(scheduledAction);
			return scheduledAction.Task;
		}

		public Task Delay(TimeSpan timeSpan, CancellationToken token = default)
		{
			return Delay((int)timeSpan.TotalMilliseconds, token);
		}

		public void Tick(float deltaTime)
		{
			lock (mLockObject)
			{
				SecondsSinceStart += deltaTime;

				foreach (var scheduledAction in mScheduledActions.Where(sa => sa.CanBeTriggered(SecondsSinceStart)))
				{
					mScheduledActionsToPrune.Add(scheduledAction);
					mScheduledActionsToTrigger.Add(scheduledAction);
				}

				foreach (var scheduledAction in mScheduledActionsToPrune)
					mScheduledActions.Remove(scheduledAction);
				mScheduledActionsToPrune.Clear();

				foreach (var scheduledAction in mScheduledActionsToTrigger)
					scheduledAction.Trigger();
				mScheduledActionsToTrigger.Clear();
			}
		}
	}
}