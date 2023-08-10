namespace Industries.Exceptions
{
	public class MaxIndustryLevelReachedException : IndustryLevelException
	{
		public MaxIndustryLevelReachedException(int level, string message) : base(level, message)
		{
		}
	}
}