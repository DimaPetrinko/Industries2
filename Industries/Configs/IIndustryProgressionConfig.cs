namespace Industries.Configs
{
	public interface IIndustryProgressionConfig
	{
		int MaxLevel { get; }
		IIndustryLevelConfig GetConfigForLevel(int level);
	}
}