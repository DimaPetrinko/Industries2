using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Time.Implementation;

namespace Core.Time
{
	public class TimeProvider : ITickable
	{
		private readonly List<ScheduledAction> mScheduledActions = new List<ScheduledAction>();
		private readonly List<ScheduledAction> mScheduledActionsToPrune = new List<ScheduledAction>();
		private readonly List<ScheduledAction> mScheduledActionsToTrigger = new List<ScheduledAction>();
		private readonly object mLockObject = new object();

		public double SecondsSinceStart { get; private set; }

		public Task Delay(int milliseconds, Action<float> onTick, CancellationToken token = default)
		{
			var scheduledAction = new ScheduledAction(
				SecondsSinceStart,
				milliseconds / 1000f,
				onTick,
				sa => mScheduledActionsToPrune.Add(sa),
				token);
			mScheduledActions.Add(scheduledAction);
			return scheduledAction.Task;
		}

		public Task Delay(int milliseconds, CancellationToken token = default)
		{
			return Delay(milliseconds, null, token);
		}

		public Task Delay(TimeSpan timeSpan, Action<float> onTick, CancellationToken token = default)
		{
			return Delay((int)timeSpan.TotalMilliseconds, onTick, token);
		}

		public Task Delay(TimeSpan timeSpan, CancellationToken token = default)
		{
			return Delay(timeSpan, null, token);
		}

		public void Tick(float deltaTime)
		{
			lock (mLockObject)
			{
				foreach (var scheduledAction in mScheduledActions)
				{
					scheduledAction.Tick(SecondsSinceStart);
					if (scheduledAction.CanBeTriggered(SecondsSinceStart))
					{
						mScheduledActionsToPrune.Add(scheduledAction);
						mScheduledActionsToTrigger.Add(scheduledAction);
					}
				}

				foreach (var scheduledAction in mScheduledActionsToPrune)
					mScheduledActions.Remove(scheduledAction);
				mScheduledActionsToPrune.Clear();

				foreach (var scheduledAction in mScheduledActionsToTrigger)
					scheduledAction.Trigger();
				mScheduledActionsToTrigger.Clear();

				SecondsSinceStart += deltaTime;
			}
		}
	}
}