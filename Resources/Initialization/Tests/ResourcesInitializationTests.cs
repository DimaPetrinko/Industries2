using System.Threading.Tasks;
using Core.Initialization.Implementation;
using NUnit.Framework;
using Resources.Configs;
using Resources.Factories;

namespace Resources.Initialization.Tests
{
	internal class ResourcesInitializationTests
	{
		[Test]
		public async Task FactoriesInitializer_BindsFactory()
		{
			var container = new InitializationContainer();
			var initializer = new ResourcesFactoriesInitializer();

			await initializer.Run(container);

			var factory = container.Get<IResourceConfigsFactory>();
			Assert.IsNotNull(factory);
		}

		[Test]
		public async Task ConfigsInitializer_BindsResourcesConfig()
		{
			var container = new InitializationContainer();
			var factoriesInitializer = new ResourcesFactoriesInitializer();
			await factoriesInitializer.Run(container);

			var configsInitializer = new ResourcesConfigsInitializer();

			await configsInitializer.Run(container);

			var resourcesConfig = container.Get<IResourcesConfig>();
			Assert.IsNotNull(resourcesConfig);
		}
	}
}