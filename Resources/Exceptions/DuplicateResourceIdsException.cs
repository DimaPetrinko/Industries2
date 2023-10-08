using System;

namespace Resources.Exceptions
{
	public class DuplicateResourceIdsException : Exception
	{
		public short Id { get; }

		public DuplicateResourceIdsException(short id)
			: base($"Resource with the same id {id} has been already added")
		{
			Id = id;
		}
	}
}