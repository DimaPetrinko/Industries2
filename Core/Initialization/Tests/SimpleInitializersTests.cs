using System.Threading.Tasks;
using Core.Initialization.Implementation;
using NUnit.Framework;

namespace Core.Initialization.Tests
{
	internal class SimpleInitializersTests
	{
		[Test]
		public async Task Run_ExecutesAction()
		{
			var container = new InitializationContainer();

			var executed = false;
			var simpleInitializer = new SimpleInitializer(c => executed = true);

			await simpleInitializer.Run(container);

			Assert.IsTrue(executed);
		}

		[Test]
		public async Task AsyncRun_ExecutesAsyncAction()
		{
			var container = new InitializationContainer();

			var executed = false;
			var simpleAsyncInitializer = new SimpleAsyncInitializer(async c =>
			{
				await Task.Delay(100);
				executed = true;
			});

			await simpleAsyncInitializer.Run(container);

			Assert.IsTrue(executed);
		}
	}
}