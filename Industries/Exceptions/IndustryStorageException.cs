using System;
using System.Collections.Generic;
using Resources;

namespace Industries.Exceptions
{
	public abstract class IndustryStorageException : Exception
	{
		public IEnumerable<ResourcePackage> Resources { get; }

		public IndustryStorageException(IEnumerable<ResourcePackage> resources, string message) : base(message)
		{
			Resources = resources;
		}
	}
}