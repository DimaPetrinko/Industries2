using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Time;
using Core.Time.Implementation;
using Industries.Configs;
using Industries.Configs.Implementation;
using Industries.Data;
using Industries.Data.Implementation;
using Industries.Exceptions;
using Industries.Model.Implementation;
using Items;
using NUnit.Framework;
using Resources;
using Resources.Configs;
using Resources.Configs.Implementation;

namespace Industries.Model.Tests
{
	internal class IndustryTests
	{
		private IIndustry mIndustry;
		private IIndustryProgressionMutableData mProgressionData;
		private IIndustryStorageMutableData mInputStorageData;
		private IIndustryStorageMutableData mOutputStorageData;
		private CancellationTokenSource mCts;

		[SetUp]
		public void SetUp()
		{
			var progressionConfig = CreateProgressionConfig();
			var itemLoadingTimesConfig = CreateLoadingTimesConfig();

			var recipe = CreateRecipe();

			mProgressionData = new IndustryProgressionData();
			mInputStorageData = new IndustryStorageData();
			mOutputStorageData = new IndustryStorageData();

			mIndustry = CreateIndustry(progressionConfig, itemLoadingTimesConfig, recipe);

			mCts = new CancellationTokenSource();

			Time.RegisterTimeProvider(new TestTimeProvider());
		}

		private static IIndustryProgressionConfig CreateProgressionConfig()
		{
			var levelConfigs = new[]
			{
				new BinaryIndustryLevelConfig(
					5f,
					1.5f,
					1.3f,
					1.6f,
					1.4f,
					50,
					50
				),
				new BinaryIndustryLevelConfig(
					4f,
					1.3f,
					1.1f,
					1.4f,
					1.2f,
					60,
					60
				),
				new BinaryIndustryLevelConfig(
					3f,
					1.1f,
					0.9f,
					1.2f,
					1.0f,
					70,
					70
				),
				new BinaryIndustryLevelConfig(
					2f,
					0.9f,
					0.7f,
					1.0f,
					0.8f,
					80,
					80
				)
			};
			var progressionConfig = new BinaryIndustryProgressionConfig(levelConfigs);
			return progressionConfig;
		}

		private static IItemsLoadingTimeConfig CreateLoadingTimesConfig()
		{
			var timings = new Dictionary<ItemType, float>
			{
				{ ItemType.Wood, 3f },
				{ ItemType.Planks, 2.5f },
				{ ItemType.Coal, 2f },
				{ ItemType.Ore, 2f },
				{ ItemType.Metal, 3f },
				{ ItemType.Instruments, 1.5f },
				{ ItemType.Goods, 0.1f }
			};
			var itemLoadingTimesConfig = new BinaryItemsLoadingTimeConfig(timings);
			return itemLoadingTimesConfig;
		}

		private static Recipe CreateRecipe()
		{
			return new Recipe(
				new[] { new ResourcePackage(ItemType.Wood, 1) },
				new[] { new ResourcePackage(ItemType.Planks, 2) });
		}

		private Industry CreateIndustry(
			IIndustryProgressionConfig progressionConfig,
			IItemsLoadingTimeConfig itemLoadingTimesConfig,
			Recipe recipe
		)
		{
			return new Industry(
				mProgressionData,
				mInputStorageData,
				mOutputStorageData,
				progressionConfig,
				itemLoadingTimesConfig,
				recipe
			);
		}

		[Test]
		public void AnyActionMethod_Throws_IfLevel0()
		{
			Assert.ThrowsAsync<InsufficientIndustryLevelException>(async () =>
			{
				await mIndustry.Produce(mCts.Token);
			});
			Assert.ThrowsAsync<InsufficientIndustryLevelException>(async () =>
			{
				await mIndustry.LoadInput(
					new[] { new ResourcePackage(ItemType.Ore, 5) },
					mCts.Token);
			});
			Assert.ThrowsAsync<InsufficientIndustryLevelException>(async () =>
			{
				await mIndustry.UnloadOutput(
					new[] { new ResourcePackage(ItemType.Ore, 5) },
					mCts.Token);
			});
		}

		[Test]
		public void AnyQueryMethodExceptCanLevelUp_ReturnsFalse_IfLevel0()
		{
			Assert.IsFalse(mIndustry.CanLoadInput(new[] { new ResourcePackage(ItemType.Ore, 5) }));
			Assert.IsFalse(mIndustry.CanUnloadOutput(new[] { new ResourcePackage(ItemType.Ore, 5) }));
		}

		[Test]
		public async Task LoadInput_AddsToInputStorage()
		{
			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(ItemType.Ore, 5) };
			await mIndustry.LoadInput(resources, mCts.Token);
			var inputStorageResources = mInputStorageData.Resources.ToArray();
			Assert.AreEqual(5, inputStorageResources[(int)ItemType.Ore - 1].Amount);
		}

		[Test]
		public async Task LoadInput_TakesTime()
		{
			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(ItemType.Ore, 5) };
			await mIndustry.LoadInput(resources, mCts.Token);
			AssertAreEqualDouble(2f * 1.5f * 5, Time.SecondsSinceStart);
		}

		[Test]
		public async Task LoadInput_Adds_UntilCancelled()
		{
			Time.ResetTimeProvider();
			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(ItemType.Goods, 5) };

			await Task.WhenAll(
				mIndustry.LoadInput(resources, mCts.Token),
				CancelAfter(200));

			var inputStorageResources = mInputStorageData.Resources.ToArray();
			Assert.AreEqual(1, inputStorageResources[(int)ItemType.Goods - 1].Amount);

			async Task CancelAfter(int milliseconds)
			{
				await Time.Delay(milliseconds);
				mCts.Cancel();
			}
		}

		[Test]
		public async Task LoadInput_Adds_UntilAble()
		{
			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(ItemType.Goods, 100) };

			await mIndustry.LoadInput(resources, mCts.Token);

			var inputStorageResources = mInputStorageData.Resources.ToArray();
			Assert.AreEqual(50, inputStorageResources[(int)ItemType.Goods - 1].Amount);
		}

		[Test]
		public void CanLoadInput_ReturnsTrue_WhenInputIsEmpty()
		{
			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(ItemType.Ore, 5) };
			Assert.IsTrue(mIndustry.CanLoadInput(resources));
		}

		[Test]
		public void CanLoadInput_ReturnsFalse_WhenInputWillBeOverCapacity()
		{
			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(ItemType.Ore, 55) };
			Assert.IsFalse(mIndustry.CanLoadInput(resources));
		}

		[Test]
		public async Task UnloadOutput_RemovesFromOutputStorage_IfHasOutput()
		{
			mProgressionData.Level = 1;
			var resources1 = new[] { new ResourcePackage(ItemType.Ore, 5) };
			var resources2 = new[] { new ResourcePackage(ItemType.Ore, 3) };
			mOutputStorageData.AddResource(resources1[0]);

			await mIndustry.UnloadOutput(resources2, mCts.Token);
			var outputStorageResources = mOutputStorageData.Resources.ToArray();
			Assert.AreEqual(2, outputStorageResources[(int)ItemType.Ore - 1].Amount);
		}

		[Test]
		public async Task UnloadOutput_TakesTime()
		{
			mProgressionData.Level = 1;
			var resources1 = new[] { new ResourcePackage(ItemType.Ore, 5) };
			var resources2 = new[] { new ResourcePackage(ItemType.Ore, 3) };
			mOutputStorageData.AddResource(resources1[0]);

			await mIndustry.UnloadOutput(resources2, mCts.Token);
			AssertAreEqualDouble(2f * 1.4f * 3, Time.SecondsSinceStart);
		}

		[Test]
		public async Task UnloadOutput_Removes_UntilCancelled()
		{
			Time.ResetTimeProvider();
			mProgressionData.Level = 1;
			var resources1 = new[] { new ResourcePackage(ItemType.Goods, 5) };
			var resources2 = new[] { new ResourcePackage(ItemType.Goods, 3) };
			mOutputStorageData.AddResource(resources1[0]);

			await Task.WhenAll(
				mIndustry.UnloadOutput(resources2, mCts.Token),
				CancelAfter(200));

			var outputStorageResources = mOutputStorageData.Resources.ToArray();
			Assert.AreEqual(4, outputStorageResources[(int)ItemType.Goods - 1].Amount);

			async Task CancelAfter(int milliseconds)
			{
				await Time.Delay(milliseconds);
				mCts.Cancel();
			}
		}

		[Test]
		public async Task UnloadOutput_Removes_UntilAble()
		{
			mProgressionData.Level = 1;
			var resources1 = new[] { new ResourcePackage(ItemType.Goods, 5) };
			var resources2 = new[] { new ResourcePackage(ItemType.Goods, 100) };
			mOutputStorageData.AddResource(resources1[0]);

			await mIndustry.UnloadOutput(resources2, mCts.Token);

			var outputStorageResources = mOutputStorageData.Resources.ToArray();
			Assert.AreEqual(0, outputStorageResources[(int)ItemType.Goods - 1].Amount);
		}

		[Test]
		public async Task UnloadOutput_Removes_UntilAbleIfRemovedWhileWaiting()
		{
			Time.ResetTimeProvider();
			mProgressionData.Level = 1;
			var resources1 = new[] { new ResourcePackage(ItemType.Goods, 5) };
			var resources2 = new[] { new ResourcePackage(ItemType.Goods, 100) };
			mOutputStorageData.AddResource(resources1[0]);

			await Task.WhenAll(
				mIndustry.UnloadOutput(resources2, mCts.Token),
				RemoveAfter(200)
			);

			var outputStorageResources = mOutputStorageData.Resources.ToArray();
			Assert.AreEqual(0, outputStorageResources[(int)ItemType.Goods - 1].Amount);

			async Task RemoveAfter(int milliseconds)
			{
				await Time.Delay(milliseconds);
				mOutputStorageData.RemoveResource(new ResourcePackage(ItemType.Goods, 4));
			}
		}

		[Test]
		public void CanUnloadOutput_ReturnsFalse_WhenEmpty()
		{
			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(ItemType.Ore, 5) };
			Assert.IsFalse(mIndustry.CanUnloadOutput(resources));
		}

		[Test]
		public void CanUnloadOutput_ReturnsTrue_WhenEnough()
		{
			mProgressionData.Level = 1;
			var resources1 = new[] { new ResourcePackage(ItemType.Ore, 5) };
			var resources2 = new[] { new ResourcePackage(ItemType.Ore, 3) };
			mOutputStorageData.AddResource(resources1[0]);
			Assert.IsTrue(mIndustry.CanUnloadOutput(resources2));
		}

		[Test]
		public void CanUnloadOutput_ReturnsFalse_WhenNotEnough()
		{
			mProgressionData.Level = 1;
			var resources1 = new[] { new ResourcePackage(ItemType.Ore, 5) };
			var resources2 = new[] { new ResourcePackage(ItemType.Ore, 8) };
			mOutputStorageData.AddResource(resources1[0]);
			Assert.IsFalse(mIndustry.CanUnloadOutput(resources2));
		}

		[Test]
		public async Task Produce_UsesInputAndStoresOutput()
		{
			mProgressionData.Level = 1;
			mInputStorageData.AddResource(new ResourcePackage(ItemType.Wood, 5));

			await mIndustry.Produce(mCts.Token);

			Assert.AreEqual(10, mOutputStorageData.Resources.ToArray()[(int)ItemType.Planks - 1].Amount);
		}

		[Test]
		public async Task Produce_TakesTime()
		{
			mProgressionData.Level = 1;
			mInputStorageData.AddResource(new ResourcePackage(ItemType.Wood, 5));

			await mIndustry.Produce(mCts.Token);

			AssertAreEqualDouble(3f * 1.3f * 5 + 5 * 5 + 2.5f * 1.6f * 10, Time.SecondsSinceStart);
		}

		[Test]
		public async Task Produce_Produces_UntilCancelled()
		{
			var recipe = new Recipe(
				new[] { new ResourcePackage(ItemType.Goods, 1) },
				new[] { new ResourcePackage(ItemType.Instruments, 2) }
			);
			mIndustry = CreateIndustry(
				CreateProgressionConfig(),
				CreateLoadingTimesConfig(),
				recipe);
			Time.ResetTimeProvider();
			mProgressionData.Level = 4;
			mInputStorageData.AddResource(new ResourcePackage(ItemType.Goods, 5));

			await Task.WhenAll(
				mIndustry.Produce(mCts.Token),
				CancelAfter(3650)
			);

			Assert.AreEqual(5, mInputStorageData.Resources.ToArray()[(int)ItemType.Goods - 1].Amount);
			Assert.AreEqual(0, mOutputStorageData.Resources.ToArray()[(int)ItemType.Instruments - 1].Amount);

			async Task CancelAfter(int milliseconds)
			{
				await Time.Delay(milliseconds);
				mCts.Cancel();
			}
		}

		private static void AssertAreEqualDouble(double expected, double actual, double epsilon = 0.01f)
		{
			Assert.Greater(actual, expected - epsilon);
			Assert.Less(actual, expected + epsilon);
		}
	}
}