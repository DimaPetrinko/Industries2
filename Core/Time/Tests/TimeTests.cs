using System;
using System.Threading.Tasks;
using Core.Time.TestTime.Implementation;
using NUnit.Framework;

namespace Core.Time.Tests
{
	public class TimeTests
	{
		[SetUp]
		public void SetUp()
		{
			Time.ResetTimeProvider();
		}
		[Test]
		public void Constructor_GivesDefaultTimeProvider()
		{
			Assert.DoesNotThrow(() =>
			{
				var _ = Time.SecondsSinceStart;
			});
		}

		[Test]
		public async Task RegisterTimeProvider_ChangesTimeProvider()
		{
			await Time.Delay(50);
			await Time.Delay(TimeSpan.FromMilliseconds(50));
			Time.RegisterTimeProvider(new TestTimeProvider());
			Assert.AreEqual(0, Time.SecondsSinceStart);
		}

		[Test]
		public async Task ResetTimeProvider_ChangesTimeProviderBackToDefault()
		{
			await Time.Delay(100);
			Time.ResetTimeProvider();
			Assert.AreEqual(0, Time.SecondsSinceStart);
		}
	}
}