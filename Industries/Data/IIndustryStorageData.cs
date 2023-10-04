using System;
using System.Collections.Generic;
using Resources;

namespace Industries.Data
{
	public interface IIndustryStorageData
	{
		event Action<ResourcePackage> ResourceAdded;
		event Action<ResourcePackage> ResourceRemoved;

		int CurrentAmount { get; }
		int CurrentCapacity { get; }
		IEnumerable<ResourcePackage> Resources { get; }
	}
}