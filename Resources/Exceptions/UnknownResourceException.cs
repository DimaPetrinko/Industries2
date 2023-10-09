using System;

namespace Resources.Exceptions
{
	public class UnknownResourceException : Exception
	{
		public short Id { get; }

		public UnknownResourceException(short id) : base($"Unknown resource with id {id}")
		{
			Id = id;
		}
	}
}