using System;
using System.Collections.Generic;
using System.Linq;
using Industries.Exceptions;
using Items;
using Resources;

namespace Industries.Data.Implementation
{
	internal class IndustryStorageData : IIndustryStorageData, IIndustryStorageMutableData
	{
		private readonly Dictionary<ItemType, int> mResourcesLookup;
		private ResourcePackage[] mResources;

		public int CurrentAmount { get; private set; }
		public int CurrentCapacity { get; set; }
		public IEnumerable<ResourcePackage> Resources => mResources;

		public IndustryStorageData()
		{
			CurrentAmount = 0;
			CurrentCapacity = 0;
			mResourcesLookup = GetDefaultResourcesLookup();
			ActualizeResourcesArray();
		}

		private static Dictionary<ItemType, int> GetDefaultResourcesLookup()
		{
			return Enum
				.GetValues(typeof(ItemType))
				.Cast<ItemType>()
				.Where(t => t != ItemType.None)
				.ToDictionary(t => t, _ => 0);
		}


		public bool CanAddResources(IEnumerable<ResourcePackage> resources)
		{
			if (CurrentAmount >= CurrentCapacity) return false;
			var resourcesArray = resources as ResourcePackage[] ?? resources.ToArray();
			if (resourcesArray.Any(r => r.Type == ItemType.None)) return false;
			return CurrentAmount + resourcesArray.Select(r => r.Amount).Sum() <= CurrentCapacity;
		}

		public bool CanRemoveResources(IEnumerable<ResourcePackage> resources)
		{
			if (CurrentAmount <= 0) return false;
			var resourcesArray = resources as ResourcePackage[] ?? resources.ToArray();
			if (resourcesArray.Any(r => r.Type == ItemType.None)) return false;
			return resourcesArray.All(r => mResourcesLookup[r.Type] - r.Amount >= 0);
		}

		public void AddResource(ResourcePackage resource)
		{
			ThrowIfAdditionIsNotValid(resource);
			mResourcesLookup[resource.Type] += resource.Amount;
			CurrentAmount += resource.Amount;
			ActualizeResourcesArray();
		}

		private void ThrowIfAdditionIsNotValid(ResourcePackage resource)
		{
			if (CurrentAmount >= CurrentCapacity)
			{
				throw new IndustryStorageAdditionException(
					new[] { resource },
					$"Could not add {resource.ToString()} to the storage, because current capacity is exceeded");
			}

			if (resource.Type == ItemType.None)
			{
				throw new IndustryStorageAdditionException(
					new[] { resource },
					$"Could not add {resource.ToString()} to the storage, because the type is {ItemType.None}");
			}

			if (CurrentAmount + resource.Amount > CurrentCapacity)
			{
				throw new IndustryStorageAdditionException(
					new[] { resource },
					$"Could not add {resource.ToString()} to the storage, because current capacity will be exceeded");
			}
		}

		public void RemoveResource(ResourcePackage resource)
		{
			ThrowIfRemovalIsNotValid(resource);
			mResourcesLookup[resource.Type] -= resource.Amount;
			CurrentAmount -= resource.Amount;
			ActualizeResourcesArray();
		}

		private void ThrowIfRemovalIsNotValid(ResourcePackage resource)
		{
			if (CurrentAmount <= 0)
			{
				throw new IndustryStorageRemovalException(
					new[] { resource },
					$"Could not remove {resource.ToString()} from the storage, because the storage is empty");
			}

			if (resource.Type == ItemType.None)
			{
				throw new IndustryStorageRemovalException(
					new[] { resource },
					$"Could not remove {resource.ToString()} from the storage, because the type is {ItemType.None}");
			}

			if (mResourcesLookup[resource.Type] - resource.Amount < 0)
			{
				throw new IndustryStorageRemovalException(
					new[] { resource },
					$"Could not remove {resource.ToString()} from the storage, because there's an insufficient amount");
			}
		}

		private void ActualizeResourcesArray()
		{
			mResources = mResourcesLookup
				.Select(p => new ResourcePackage(p.Key, p.Value))
				.ToArray();
		}
	}
}