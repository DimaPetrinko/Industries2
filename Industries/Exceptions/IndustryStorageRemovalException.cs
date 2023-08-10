using System.Collections.Generic;
using Resources;

namespace Industries.Exceptions
{
	public class IndustryStorageRemovalException : IndustryStorageException
	{
		public IndustryStorageRemovalException(IEnumerable<ResourcePackage> resources, string message) : base(resources, message)
		{
		}
	}
}