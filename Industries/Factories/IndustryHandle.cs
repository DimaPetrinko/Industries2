using Industries.Data;
using Industries.Model;

namespace Industries.Factories
{
	public readonly struct IndustryHandle
	{
		public readonly IIndustry Industry;
		public readonly IIndustryProgression Progression;
		public readonly IIndustryStateData StateData;
		public readonly IIndustryStorageData InputStorageData;
		public readonly IIndustryStorageData OutputStorageData;
		public readonly IIndustryProgressionData ProgressionData;

		public IndustryHandle(IIndustry industry,
			IIndustryProgression progression,
			IIndustryStateData stateData,
			IIndustryStorageData inputStorageData,
			IIndustryStorageData outputStorageData,
			IIndustryProgressionData progressionData)
		{
			Industry = industry;
			Progression = progression;
			StateData = stateData;
			InputStorageData = inputStorageData;
			OutputStorageData = outputStorageData;
			ProgressionData = progressionData;
		}
	}
}