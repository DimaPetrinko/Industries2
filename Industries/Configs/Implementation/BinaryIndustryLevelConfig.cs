namespace Industries.Configs.Implementation
{
	internal class BinaryIndustryLevelConfig : IIndustryLevelConfig
	{
		public float ProductionTime { get; }
		public float InputLoadingMultiplier { get; }
		public float InputUnloadingMultiplier { get; }
		public float OutputLoadingMultiplier { get; }
		public float OutputUnloadingMultiplier { get; }
		public int InputAreaCapacity { get; }
		public int OutputAreaCapacity { get; }

		public BinaryIndustryLevelConfig(
			float productionTime,
			float inputLoadingMultiplier,
			float inputUnloadingMultiplier,
			float outputLoadingMultiplier,
			float outputUnloadingMultiplier,
			int inputAreaCapacity,
			int outputAreaCapacity
		)
		{
			ProductionTime = productionTime;
			InputLoadingMultiplier = inputLoadingMultiplier;
			InputUnloadingMultiplier = inputUnloadingMultiplier;
			OutputLoadingMultiplier = outputLoadingMultiplier;
			OutputUnloadingMultiplier = outputUnloadingMultiplier;
			InputAreaCapacity = inputAreaCapacity;
			OutputAreaCapacity = outputAreaCapacity;
		}
	}
}