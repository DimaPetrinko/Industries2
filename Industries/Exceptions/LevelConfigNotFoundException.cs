using System;

namespace Industries.Exceptions
{
	// TODO: move to core
	public class LevelConfigNotFoundException : Exception
	{
		public int Level { get; }

		public LevelConfigNotFoundException(int level, string message) : base(message)
		{
			Level = level;
		}
	}
}