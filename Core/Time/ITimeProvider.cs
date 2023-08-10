using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Time
{
	public interface ITimeProvider
	{
		double SecondsSinceStart { get; }
		Task Delay(int milliseconds, CancellationToken token = default);
		Task Delay(TimeSpan timeSpan, CancellationToken token = default);
	}
}