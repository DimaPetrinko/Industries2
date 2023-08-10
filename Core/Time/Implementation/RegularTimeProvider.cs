using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Time.Implementation
{
	internal class RegularTimeProvider : ITimeProvider
	{
		private readonly DateTime mStartTime;

		public double SecondsSinceStart => (DateTime.UtcNow - mStartTime).TotalSeconds;

		public RegularTimeProvider()
		{
			mStartTime = DateTime.UtcNow;
		}

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