using System.Collections.Generic;
using System.Linq;
using Industries.Exceptions;

namespace Industries.Configs.Implementation
{
	public class BinaryIndustryProgressionConfig : IIndustryProgressionConfig
	{
		private readonly IIndustryLevelConfig[] mLevelConfigs;

		public BinaryIndustryProgressionConfig(IEnumerable<IIndustryLevelConfig> levelConfigs)
		{
			mLevelConfigs = levelConfigs.ToArray();
		}

		public int MaxLevel => mLevelConfigs.Length;

		public IIndustryLevelConfig GetConfigForLevel(int level)
		{
			if (level <= 0 || level > mLevelConfigs.Length)
				throw new LevelConfigNotFoundException(level, $"Level {level} is not present in this config");

			return mLevelConfigs[level - 1];
		}
	}
}