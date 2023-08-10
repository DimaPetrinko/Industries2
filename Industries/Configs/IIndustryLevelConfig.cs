namespace Industries.Configs
{
	public interface IIndustryLevelConfig
	{
		float ProductionTime { get; }
		float InputLoadingMultiplier { get; }
		float InputUnloadingMultiplier { get; }
		float OutputLoadingMultiplier { get; }
		float OutputUnloadingMultiplier { get; }
		int InputAreaCapacity { get; }
		int OutputAreaCapacity { get; }
	}
}