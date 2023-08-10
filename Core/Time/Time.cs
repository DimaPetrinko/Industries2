using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Time.Implementation;

namespace Core.Time
{
	public static class Time
	{
		private static ITimeProvider sTimeProvider = new RegularTimeProvider();

		public static double SecondsSinceStart => sTimeProvider.SecondsSinceStart;
		public static Task Delay(int milliseconds, CancellationToken token = default)
		{
			return sTimeProvider.Delay(milliseconds, token);
		}

		public static Task Delay(TimeSpan timeSpan, CancellationToken token = default)
		{
			return sTimeProvider.Delay(timeSpan, token);
		}

		public static void RegisterTimeProvider(ITimeProvider timeProvider)
		{
			sTimeProvider = timeProvider;
		}

		public static void ResetTimeProvider()
		{
			sTimeProvider = new RegularTimeProvider();
		}
	}
}