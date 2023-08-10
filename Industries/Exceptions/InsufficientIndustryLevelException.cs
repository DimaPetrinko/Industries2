namespace Industries.Exceptions
{
	public class InsufficientIndustryLevelException : IndustryLevelException
	{
		public InsufficientIndustryLevelException(int level, string message) : base(level, message)
		{
		}
	}
}