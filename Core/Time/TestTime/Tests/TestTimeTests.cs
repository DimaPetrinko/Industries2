using System;
using System.Threading.Tasks;
using Core.Time.TestTime.Implementation;
using NUnit.Framework;

namespace Core.Time.TestTime.Tests
{
	public class TestTimeTests
	{
		private ITimeProvider mTime;

		[SetUp]
		public void SetUp()
		{
			mTime = new TestTimeProvider();
		}

		[Test]
		public async Task Delay_ProgressesSecondsFromStart()
		{
			await mTime.Delay(100);
			AssertAreEqualDouble(0.1f, mTime.SecondsSinceStart);
			await mTime.Delay(TimeSpan.FromMilliseconds(100));
			AssertAreEqualDouble(0.2f, mTime.SecondsSinceStart);
		}

		[Test]
		public async Task Delay_AddsUpTimes_IFInWhenAll()
		{
			var delay1 = mTime.Delay(100);
			var delay2 = mTime.Delay(200);
			await Task.WhenAll(delay1, delay2);
			AssertAreEqualDouble(0.3f, mTime.SecondsSinceStart);
		}

		[Test]
		public async Task Delay_AddsUpTimes_IFInWhenAny()
		{
			var delay1 = mTime.Delay(100);
			var delay2 = mTime.Delay(200);
			await Task.WhenAny(delay1, delay2);
			AssertAreEqualDouble(0.3f, mTime.SecondsSinceStart);
		}

		private void AssertAreEqualDouble(double expected, double actual, double epsilon = 0.01f)
		{
			Assert.Greater(actual, expected - epsilon);
			Assert.Less(actual, expected + epsilon);
		}
	}
}