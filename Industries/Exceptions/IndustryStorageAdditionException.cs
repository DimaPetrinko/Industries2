using System.Collections.Generic;
using Resources;

namespace Industries.Exceptions
{
	public class IndustryStorageAdditionException : IndustryStorageException
	{
		public IndustryStorageAdditionException(IEnumerable<ResourcePackage> resources, string message) : base(resources, message)
		{
		}
	}
}