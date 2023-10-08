using System;
using System.Collections.Generic;
using System.Linq;
using Industries.Exceptions;
using Resources;
using Resources.Configs;
using Resources.Exceptions;

namespace Industries.Data.Implementation
{
	internal class IndustryStorageData : IIndustryStorageData, IIndustryStorageMutableData
	{
		public event Action<ResourcePackage> ResourceAdded;
		public event Action<ResourcePackage> ResourceRemoved;

		private readonly IResourcesConfig mResourcesConfig;
		private readonly Dictionary<short, int> mResourcesLookup;
		private ResourcePackage[] mResources;

		public int CurrentAmount { get; private set; }
		public int CurrentCapacity { get; set; }
		public IEnumerable<ResourcePackage> Resources => mResources;

		public IndustryStorageData(IResourcesConfig resourcesConfig)
		{
			mResourcesConfig = resourcesConfig;

			CurrentAmount = 0;
			CurrentCapacity = 0;
			mResourcesLookup = GetDefaultResourcesLookup(resourcesConfig);
			ActualizeResourcesArray();
		}

		private static Dictionary<short, int> GetDefaultResourcesLookup(IResourcesConfig resourcesConfig)
		{
			return resourcesConfig.AllResourceConfigs.ToDictionary(p => p.Key, _ => 0);
		}


		public bool CanAddResources(IEnumerable<ResourcePackage> resources)
		{
			var resourcesArray = resources as ResourcePackage[] ?? resources.ToArray();
			if (resourcesArray.Any(r => !mResourcesLookup.ContainsKey(r.ResourceId))) return false;
			return CurrentAmount + resourcesArray.Select(r => r.Amount).Sum() <= CurrentCapacity;
		}

		public bool CanRemoveResources(IEnumerable<ResourcePackage> resources)
		{
			var resourcesArray = resources as ResourcePackage[] ?? resources.ToArray();
			if (resourcesArray.Any(r => !mResourcesLookup.ContainsKey(r.ResourceId))) return false;
			return resourcesArray.All(r => mResourcesLookup[r.ResourceId] - r.Amount >= 0);
		}

		public void AddResource(ResourcePackage resource)
		{
			ThrowIfAdditionIsNotValid(resource);
			mResourcesLookup[resource.ResourceId] += resource.Amount;
			CurrentAmount += resource.Amount;
			ActualizeResourcesArray();
			ResourceAdded?.Invoke(resource);
		}

		private void ThrowIfAdditionIsNotValid(ResourcePackage resource)
		{
			if (CurrentAmount >= CurrentCapacity)
			{
				throw new IndustryStorageAdditionException(
					new[] { resource },
					$"Could not add {resource.ToString(GetResourceName(resource.ResourceId))} to the storage, because current capacity is exceeded");
			}

			if (!mResourcesLookup.ContainsKey(resource.ResourceId))
			{
				throw new IndustryStorageAdditionException(
					new[] { resource },
					$"Could not add {resource.ToString(GetResourceName(resource.ResourceId))} to the storage, because resource id {resource.ResourceId} is unknown");
			}

			if (CurrentAmount + resource.Amount > CurrentCapacity)
			{
				throw new IndustryStorageAdditionException(
					new[] { resource },
					$"Could not add {resource.ToString(GetResourceName(resource.ResourceId))} to the storage, because current capacity will be exceeded");
			}
		}

		public void RemoveResource(ResourcePackage resource)
		{
			ThrowIfRemovalIsNotValid(resource);
			mResourcesLookup[resource.ResourceId] -= resource.Amount;
			CurrentAmount -= resource.Amount;
			ActualizeResourcesArray();
			ResourceRemoved?.Invoke(resource);
		}

		private void ThrowIfRemovalIsNotValid(ResourcePackage resource)
		{
			if (CurrentAmount <= 0)
			{
				throw new IndustryStorageRemovalException(
					new[] { resource },
					$"Could not remove {resource.ToString(GetResourceName(resource.ResourceId))} from the storage, because the storage is empty");
			}

			if (!mResourcesLookup.ContainsKey(resource.ResourceId))
			{
				throw new IndustryStorageRemovalException(
					new[] { resource },
					$"Could not remove {resource.ToString(GetResourceName(resource.ResourceId))} from the storage, because resource id {resource.ResourceId} is unknown");
			}

			if (mResourcesLookup[resource.ResourceId] - resource.Amount < 0)
			{
				throw new IndustryStorageRemovalException(
					new[] { resource },
					$"Could not remove {resource.ToString(GetResourceName(resource.ResourceId))} from the storage, because there's an insufficient amount");
			}
		}

		private void ActualizeResourcesArray()
		{
			mResources = mResourcesLookup
				.Select(p => new ResourcePackage(p.Key, p.Value))
				.ToArray();
		}

		private string GetResourceName(short resourceId)
		{
			try
			{
				return mResourcesConfig.GetResourceConfig(resourceId).Name;
			}
			catch (UnknownResourceException)
			{
				return "Unknown";
			}
		}
	}
}