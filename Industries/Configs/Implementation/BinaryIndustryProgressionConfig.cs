using System;
using System.Collections.Generic;
using System.Linq;
using Core.Exceptions;

namespace Industries.Configs.Implementation
{
	internal class BinaryIndustryProgressionConfig : IIndustryProgressionConfig
	{
		private readonly IIndustryLevelConfig[] mLevelConfigs;

		public BinaryIndustryProgressionConfig(IEnumerable<IIndustryLevelConfig> levelConfigs)
		{
			if (levelConfigs == null)
			{
				throw new ArgumentException("Level configs cannot be null");
			}
			var configs = levelConfigs.ToArray();
			const int maxCount = byte.MaxValue - 1;
			if (configs.Length > maxCount)
			{
				throw new ArgumentException($"Level configs count cannot be greater than {maxCount}");
			}
			if (configs.Any(c => c == null))
			{
				throw new ArgumentException($"Some level configs are null");
			}

			mLevelConfigs = configs;
		}

		public byte MaxLevel => (byte)mLevelConfigs.Length;

		public IIndustryLevelConfig GetConfigForLevel(byte level)
		{
			if (level == 0 || level > MaxLevel)
				throw new LevelConfigNotFoundException(level, $"Level {level} is not present in this config");

			return mLevelConfigs[level - 1];
		}
	}
}