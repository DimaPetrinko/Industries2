using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Time
{
	public static class Time
	{
		private static ITimeModel sTimeModel;
		private static TimeProvider sTimeProvider;

		public static double SecondsSinceStart => sTimeProvider.SecondsSinceStart;

		public static float Scale
		{
			get => sTimeModel.Scale;
			set => sTimeModel.Scale = value;
		}

		public static void Initialize(ITimeModel timeModel, TimeProvider timeProvider)
		{
			sTimeModel?.Stop();
			sTimeModel = timeModel;
			sTimeProvider = timeProvider;
		}

		public static void Start()
		{
			sTimeModel.Start();
		}

		public static void Stop()
		{
			sTimeModel.Stop();
		}

		public static Task Delay(int milliseconds, Action<float> onTick, CancellationToken token = default)
		{
			return sTimeProvider.Delay(milliseconds, onTick, token);
		}

		public static Task Delay(int milliseconds, CancellationToken token = default)
		{
			return sTimeProvider.Delay(milliseconds, token);
		}

		public static Task Delay(TimeSpan timeSpan, Action<float> onTick, CancellationToken token = default)
		{
			return sTimeProvider.Delay(timeSpan, onTick, token);
		}

		public static Task Delay(TimeSpan timeSpan, CancellationToken token = default)
		{
			return sTimeProvider.Delay(timeSpan, token);
		}
	}
}