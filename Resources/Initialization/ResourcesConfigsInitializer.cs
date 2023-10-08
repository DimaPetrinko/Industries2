using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Initialization;
using Resources.Configs;
using Resources.Factories;

namespace Resources.Initialization
{
	public class ResourcesConfigsInitializer : IInitializer
	{
		private IResourceConfigsFactory mResourceConfigsFactory;

		public Task Run(IInitializationContainer container)
		{
			mResourceConfigsFactory = container.Get<IResourceConfigsFactory>();

			var configs = CreateItemConfigs();
			var resourcesConfig = mResourceConfigsFactory.CreateResourcesConfig(configs);

			container.Bind(resourcesConfig);

			return Task.CompletedTask;
		}

		private IEnumerable<IResourceConfig> CreateItemConfigs()
		{
			var configs = new[]
			{
				mResourceConfigsFactory.CreateConfig(1, "Wood", 3f),
				mResourceConfigsFactory.CreateConfig(2, "Planks", 2f),
				mResourceConfigsFactory.CreateConfig(3, "Coal", 4f),
				mResourceConfigsFactory.CreateConfig(4, "Ore", 4f),
				mResourceConfigsFactory.CreateConfig(5, "Metal", 2f),
				mResourceConfigsFactory.CreateConfig(6, "Instruments", 1.5f),
				mResourceConfigsFactory.CreateConfig(7, "Goods", 1.5f)
			};
			return configs;
		}
	}
}