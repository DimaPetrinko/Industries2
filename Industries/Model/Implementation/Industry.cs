using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Time;
using Industries.Configs;
using Industries.Data;
using Industries.Exceptions;
using Resources;
using Resources.Configs;

namespace Industries.Model.Implementation
{
	internal class Industry : IIndustry
	{
		private readonly IIndustryProgressionMutableData mProgressionData;
		private readonly IIndustryStorageMutableData mInputStorageData;
		private readonly IIndustryStorageMutableData mOutputStorageData;
		private readonly IIndustryProgressionConfig mProgressionConfig;
		private readonly IItemsLoadingTimeConfig mItemsLoadingTimeConfig;
		private readonly Recipe mProductionRecipe;

		private IIndustryLevelConfig mCurrentLevelConfig;

		private bool IsLocked => mProgressionData.Level == 0;

		public Industry(
			IIndustryProgressionMutableData progressionData,
			IIndustryStorageMutableData inputStorageData,
			IIndustryStorageMutableData outputStorageData,
			IIndustryProgressionConfig progressionConfig,
			IItemsLoadingTimeConfig itemsLoadingTimeConfig,
			Recipe productionRecipe
		)
		{
			mProgressionData = progressionData;
			mInputStorageData = inputStorageData;
			mOutputStorageData = outputStorageData;
			mProgressionConfig = progressionConfig;
			mItemsLoadingTimeConfig = itemsLoadingTimeConfig;
			mProductionRecipe = productionRecipe;

			mProgressionData.LevelChanged += OnLevelChanged;
		}

		private void OnLevelChanged(int level)
		{
			mCurrentLevelConfig = mProgressionConfig.GetConfigForLevel(level);
			mInputStorageData.CurrentCapacity = mCurrentLevelConfig.InputAreaCapacity;
			mOutputStorageData.CurrentCapacity = mCurrentLevelConfig.OutputAreaCapacity;
		}

		public async Task Produce(CancellationToken token)
		{
			ThrowIfLocked();

			while (mInputStorageData.CanRemoveResources(mProductionRecipe.From) &&
			       mOutputStorageData.CanAddResources(mProductionRecipe.To))
			{
				var undoActions = new List<Action>();

				try
				{
					var inputLoadingTimes = GetLoadingTimesForResources(
						mProductionRecipe.From,
						mCurrentLevelConfig.InputUnloadingMultiplier);
					await PerformLoadingAction(
						inputLoadingTimes,
						resource => mInputStorageData.CanRemoveResources(new[] { resource }),
						resource =>
						{
							undoActions.Add(() => mInputStorageData.AddResource(resource));
							mInputStorageData.RemoveResource(resource);
						},
						token);

					await Time.Delay(TimeSpan.FromSeconds(mCurrentLevelConfig.ProductionTime), token);

					var outputLoadingTimes = GetLoadingTimesForResources(
						mProductionRecipe.To,
						mCurrentLevelConfig.OutputLoadingMultiplier);
					await PerformLoadingAction(
						outputLoadingTimes,
						resource => mOutputStorageData.CanAddResources(new[] { resource }),
						resource =>
						{
							undoActions.Add(() => mOutputStorageData.RemoveResource(resource));
							mOutputStorageData.AddResource(resource);
						},
						token);
				}
				catch (OperationCanceledException)
				{
					foreach (var undoAction in undoActions)
					{
						undoAction();
					}

					break;
				}
			}
		}

		public async Task LoadInput(IEnumerable<ResourcePackage> resources, CancellationToken token)
		{
			ThrowIfLocked();
			var resourcesArray = resources.ToArray();

			var loadingTimes = GetLoadingTimesForResources(
				resourcesArray,
				mCurrentLevelConfig.InputLoadingMultiplier);

			try
			{
				await PerformLoadingAction(
					loadingTimes,
					resource => mInputStorageData.CanAddResources(new[] { resource }),
					resource => mInputStorageData.AddResource(resource),
					token
				);
			}
			catch (OperationCanceledException)
			{
			}
		}

		public async Task UnloadOutput(IEnumerable<ResourcePackage> requestedResources, CancellationToken token)
		{
			ThrowIfLocked();
			var resourcesArray = requestedResources.ToArray();
			var loadingTimes = GetLoadingTimesForResources(
				resourcesArray,
				mCurrentLevelConfig.OutputUnloadingMultiplier);

			try
			{
				await PerformLoadingAction(
					loadingTimes,
					resource => mOutputStorageData.CanRemoveResources(new[] { resource }),
					resource => mOutputStorageData.RemoveResource(resource),
					token);
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void ThrowIfLocked()
		{
			if (IsLocked)
				throw new InsufficientIndustryLevelException(
					mProgressionData.Level,
					"Cannot perform this action. The level must be at least 1");
		}

		private IEnumerable<ResourcePackageLoadingTime> GetLoadingTimesForResources(
			IEnumerable<ResourcePackage> resourcesArray,
			float multiplier
		)
		{
			return resourcesArray.Select(p => GetResourceLoadingTime(p, multiplier));
		}

		private ResourcePackageLoadingTime GetResourceLoadingTime(ResourcePackage resource, float timeMultiplier)
		{
			return new ResourcePackageLoadingTime(
				resource,
				mItemsLoadingTimeConfig.GetLoadingTimeForItemType(resource.Type) * timeMultiplier);
		}

		private static async Task PerformLoadingAction(
			IEnumerable<ResourcePackageLoadingTime> loadingTimes,
			Func<ResourcePackage, bool> checkAction,
			Action<ResourcePackage> loadAction,
			CancellationToken token
		)
		{
			foreach (var resourceLoadingTime in loadingTimes)
			{
				await PerformLoadingActionForEachResourcePiece(
					resourceLoadingTime.Resource,
					resourceLoadingTime.OnePieceLoadingTime,
					checkAction,
					loadAction,
					token
				);
			}
		}

		private static async Task PerformLoadingActionForEachResourcePiece(
			ResourcePackage resource,
			float onePieceLoadingTime,
			Func<ResourcePackage, bool> checkAction,
			Action<ResourcePackage> loadAction,
			CancellationToken token
		)
		{
			for (var i = 0; i < resource.Amount; i++)
			{
				var onePieceOfResource = new ResourcePackage(resource.Type, 1);
				if (!checkAction(onePieceOfResource)) return;
				await Time.Delay(TimeSpan.FromSeconds(onePieceLoadingTime), token);
				if (!checkAction(onePieceOfResource)) return;
				loadAction(onePieceOfResource);
			}
		}

		public bool CanLoadInput(IEnumerable<ResourcePackage> resources)
		{
			return !IsLocked && mInputStorageData.CanAddResources(resources);
		}

		public bool CanUnloadOutput(IEnumerable<ResourcePackage> requestedResources)
		{
			return !IsLocked && mOutputStorageData.CanRemoveResources(requestedResources);
		}
	}
}