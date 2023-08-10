using System.Collections.Generic;
using Resources;

namespace Industries.Data
{
	public interface IIndustryStorageMutableData : IIndustryStorageData
	{
		int CurrentCapacity { get; set; }
		bool CanAddResources(IEnumerable<ResourcePackage> resources);
		bool CanRemoveResources(IEnumerable<ResourcePackage> resources);
		void AddResource(ResourcePackage resource);
		void RemoveResource(ResourcePackage resource);
	}
}