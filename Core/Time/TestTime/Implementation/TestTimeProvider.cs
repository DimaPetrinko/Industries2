using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Time.TestTime.Implementation
{
	public class TestTimeProvider : ITimeProvider
	{
		public double SecondsSinceStart { get; private set; }

		public Task Delay(int milliseconds, CancellationToken token = default)
		{
			SecondsSinceStart += milliseconds / 1000f;
			return Task.CompletedTask;
		}

		public Task Delay(TimeSpan timeSpan, CancellationToken token = default)
		{
			SecondsSinceStart += timeSpan.TotalSeconds;
			return Task.CompletedTask;
		}
	}
}