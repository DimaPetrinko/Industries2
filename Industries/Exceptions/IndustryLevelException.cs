using System;

namespace Industries.Exceptions
{
	public abstract class IndustryLevelException : Exception
	{
		public int Level { get; }

		protected IndustryLevelException(int level, string message) : base(message)
		{
			Level = level;
		}
	}
}