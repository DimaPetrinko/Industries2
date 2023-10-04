using System;

namespace Industries.Exceptions
{
	public abstract class IndustryLevelException : Exception
	{
		public byte Level { get; }

		protected IndustryLevelException(byte level, string message) : base(message)
		{
			Level = level;
		}
	}
}