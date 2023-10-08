using Industries.Configs;
using Resources.Configs;

namespace Industries.Factories
{
	public interface IIndustryFactory
	{
		IndustryHandle Create(
			short id,
			Recipe productionRecipe,
			IIndustryProgressionConfig progressionConfig,
			IResourcesConfig resourcesConfig
		);
	}
}