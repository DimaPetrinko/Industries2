using Industries.Configs;
using Industries.Data.Implementation;
using Industries.Model.Implementation;
using Resources.Configs;

namespace Industries.Factories.Implementation
{
	internal class IndustryFactory : IIndustryFactory
	{
		public IndustryHandle Create(
			short id,
			Recipe productionRecipe,
			IIndustryProgressionConfig progressionConfig,
			IResourcesConfig resourcesConfig
		)
		{
			var stateData = new IndustryStateData(id);
			var progressionData = new IndustryProgressionData();
			var inputStorageData = new IndustryStorageData(resourcesConfig);
			var outputStorageData = new IndustryStorageData(resourcesConfig);

			var progression = new IndustryProgression(progressionData, progressionConfig);

			var industry = new Industry(
				stateData,
				progressionData,
				inputStorageData,
				outputStorageData,
				progressionConfig,
				resourcesConfig,
				productionRecipe
			);

			var handle = new IndustryHandle(
				industry,
				progression,
				stateData,
				inputStorageData,
				outputStorageData,
				progressionData
			);

			return handle;
		}
	}
}