namespace Core.Configs
{
	public interface IProgressionConfig<out TLevelConfig>
	{
		byte MaxLevel { get; }
		TLevelConfig GetConfigForLevel(byte level);
	}
}