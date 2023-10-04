using Industries.Configs;
using Industries.Data;
using Industries.Exceptions;

namespace Industries.Model.Implementation
{
	internal class IndustryProgression : IIndustryProgression
	{
		private readonly IIndustryProgressionMutableData mData;
		private readonly IIndustryProgressionConfig mConfig;

		public IndustryProgression(
			IIndustryProgressionMutableData data,
			IIndustryProgressionConfig config
		)
		{
			mData = data;
			mConfig = config;
		}

		public bool CanLevelUp()
		{
			return mData.Level != mConfig.MaxLevel;
		}

		public void LevelUp()
		{
			if (mData.Level == mConfig.MaxLevel)
				throw new MaxIndustryLevelReachedException(
					(byte)(mData.Level + 1),
					$"Already at max level of {mConfig.MaxLevel}! Cannot level up further.");
			mData.Level++;
		}
	}
}