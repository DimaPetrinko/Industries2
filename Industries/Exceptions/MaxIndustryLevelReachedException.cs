namespace Industries.Exceptions
{
	public class MaxIndustryLevelReachedException : IndustryLevelException
	{
		public MaxIndustryLevelReachedException(byte level, string message) : base(level, message)
		{
		}
	}
}