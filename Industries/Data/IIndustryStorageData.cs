using System.Collections.Generic;
using Resources;

namespace Industries.Data
{
	public interface IIndustryStorageData
	{
		int CurrentAmount { get; }
		int CurrentCapacity { get; }
		IEnumerable<ResourcePackage> Resources { get; }
	}
}