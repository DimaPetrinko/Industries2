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
			IItemsLoadingTimeConfig itemsLoadingTimeConfig
		)
		{
			var stateData = new IndustryStateData(id);
			var progressionData = new IndustryProgressionData();
			var inputStorageData = new IndustryStorageData();
			var outputStorageData = new IndustryStorageData();

			var progression = new IndustryProgression(progressionData, progressionConfig);

			var industry = new Industry(
				stateData,
				progressionData,
				inputStorageData,
				outputStorageData,
				progressionConfig,
				itemsLoadingTimeConfig,
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