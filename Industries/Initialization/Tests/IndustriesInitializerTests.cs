using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Initialization.Implementation;
using Industries.Factories;
using Industries.Initialization.Implementation;
using NUnit.Framework;

namespace Industries.Initialization.Tests
{
	internal class IndustriesInitializerTests
	{
		[Test]
		public void Constructor_Creates()
		{
			Assert.DoesNotThrow(() =>
			{
				var initializer = new IndustriesInitializer();
			});
		}

		[Test]
		public async Task Run_SavesConfigsToData()
		{
			var container = new InitializationContainer();

			Assert.IsFalse(container.Has<IDictionary<short, IndustryHandle>>());

			var configsInitializer = new IndustriesConfigsInitializer();
			await configsInitializer.Run(container);

			var industriesFactoriesInitializer = new IndustriesFactoriesInitializer();
			await industriesFactoriesInitializer.Run(container);

			var initializer = new IndustriesInitializer();
			await initializer.Run(container);

			Assert.IsTrue(container.Has<IDictionary<short, IndustryHandle>>());
			var industries = container.Get<IDictionary<short, IndustryHandle>>();
			Assert.IsNotEmpty(industries);
		}
	}
}