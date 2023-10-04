namespace Industries.Exceptions
{
	public class InsufficientIndustryLevelException : IndustryLevelException
	{
		public InsufficientIndustryLevelException(byte level, string message) : base(level, message)
		{
		}
	}
}