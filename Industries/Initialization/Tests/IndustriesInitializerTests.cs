using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Initialization.Implementation;
using Industries.Factories;
using Industries.Initialization.Implementation;
using NUnit.Framework;
using Resources.Initialization;

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

			var resourcesFactoriesInitializer = new ResourcesFactoriesInitializer();
			var industriesFactoriesInitializer = new IndustriesFactoriesInitializer();
			var resourcesConfigsInitializer = new ResourcesConfigsInitializer();
			var configsInitializer = new IndustriesConfigsInitializer();
			await resourcesFactoriesInitializer.Run(container);
			await industriesFactoriesInitializer.Run(container);
			await resourcesConfigsInitializer.Run(container);
			await configsInitializer.Run(container);

			var initializer = new IndustriesInitializer();
			await initializer.Run(container);

			Assert.IsTrue(container.Has<IDictionary<short, IndustryHandle>>());
			var industries = container.Get<IDictionary<short, IndustryHandle>>();
			Assert.IsNotEmpty(industries);
		}
	}
}