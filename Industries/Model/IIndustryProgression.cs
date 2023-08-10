namespace Industries.Model
{
	public interface IIndustryProgression
	{
		bool CanLevelUp();
		void LevelUp();
	}
}