using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Time.TaskTime.Implementation
{
	internal class TaskTimeProvider : ITimeProvider
	{
		private readonly DateTime mStartTime = DateTime.UtcNow;

		public double SecondsSinceStart => (DateTime.UtcNow - mStartTime).TotalSeconds;

		public Task Delay(int milliseconds, CancellationToken token = default)
		{
			return Task.Delay(milliseconds, token);
		}

		public Task Delay(TimeSpan timeSpan, CancellationToken token = default)
		{
			return Task.Delay(timeSpan, token);
		}
	}
}