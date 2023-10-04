using System.Threading.Tasks;
using Core.Initialization.Implementation;
using NUnit.Framework;

namespace Core.Initialization.Tests
{
	internal class ConcurrentInitializerTests
	{
		[Test]
		public async Task Run_ExecutesAllActions()
		{
			var container = new InitializationContainer();

			var triggered1 = false;
			var triggered2 = false;

			var simpleInitializer1 = new SimpleInitializer(c => triggered1 = true);
			var simpleInitializer2 = new SimpleInitializer(c => triggered2 = true);
			var concurrentInitializer = new ConcurrentInitializer(simpleInitializer1, simpleInitializer2);

			await concurrentInitializer.Run(container);

			Assert.IsTrue(triggered1);
			Assert.IsTrue(triggered2);
		}
	}
}