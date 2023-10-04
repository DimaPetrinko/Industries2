using System;

namespace Core.Exceptions
{
	public class LevelConfigNotFoundException : Exception
	{
		public byte Level { get; }

		public LevelConfigNotFoundException(byte level, string message) : base(message)
		{
			Level = level;
		}
	}
}