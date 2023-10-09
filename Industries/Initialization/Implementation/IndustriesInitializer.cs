using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Initialization;
using Industries.Configs;
using Industries.Factories;
using Resources.Configs;

namespace Industries.Initialization.Implementation
{
	public class IndustriesInitializer : IInitializer
	{
		private IIndustriesConfig mIndustriesConfig;
		private IIndustryProductionConfig mProductionConfig;
		private IIndustryProgressionConfig mProgressionConfig;
		private IResourcesConfig mResourcesConfig;
		private IIndustryFactory mFactory;

		public Task Run(IInitializationContainer container)
		{
			GatherDependencies(container);

			var industries = mIndustriesConfig
				.Industries
				.ToDictionary(pair => pair.Key, pair => CreateIndustry(pair.Key, pair.Value));

			container.Bind<IDictionary<short, IndustryHandle>>(industries);

			return Task.CompletedTask;
		}

		private void GatherDependencies(IInitializationContainer container)
		{
			mIndustriesConfig = container.Get<IIndustriesConfig>();
			mProductionConfig = container.Get<IIndustryProductionConfig>();
			mProgressionConfig = container.Get<IIndustryProgressionConfig>();
			mResourcesConfig = container.Get<IResourcesConfig>();
			mFactory = container.Get<IIndustryFactory>();
		}

		private IndustryHandle CreateIndustry(short id, IIndustryDataConfig dataConfig)
		{
			var recipe = mProductionConfig.GetRecipeById(dataConfig.RecipeId);
			var industryHandle = mFactory.Create(
				id,
				recipe,
				mProgressionConfig,
				mResourcesConfig);
			return industryHandle;
		}
	}
}