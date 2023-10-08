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
using NUnit.Framework;
using Resources;
using Resources.Configs;

namespace Industries.Model.Tests
{
	internal class IndustryTests
	{
		#region Mock classes

		private class MockResourceConfig : IResourceConfig
		{
			public short Id { get; }
			public string Name { get; }
			public float LoadingTime { get; }

			public MockResourceConfig(short id, string name, float loadingTime)
			{
				Id = id;
				Name = name;
				LoadingTime = loadingTime;
			}
		}

		private class MockResourcesConfig : IResourcesConfig
		{

			public IReadOnlyDictionary<short, IResourceConfig> AllResourceConfigs { get; }

			public MockResourcesConfig(Dictionary<short, IResourceConfig> allResourceConfigs)
			{
				AllResourceConfigs = allResourceConfigs;
			}

			public IResourceConfig GetResourceConfig(short id)
			{
				return AllResourceConfigs[id];
			}
		}

		#endregion

		private IIndustry mIndustry;
		private IIndustryStateMutableData mStateData;
		private IIndustryProgressionMutableData mProgressionData;
		private IIndustryStorageMutableData mInputStorageData;
		private IIndustryStorageMutableData mOutputStorageData;
		private CancellationTokenSource mCts;

		[SetUp]
		public void SetUp()
		{
			var progressionConfig = CreateProgressionConfig();
			var resourcesConfig = CreateResourceConfig();

			var recipe = CreateRecipe();

			mStateData = new IndustryStateData(1);
			mProgressionData = new IndustryProgressionData();
			mInputStorageData = new IndustryStorageData(resourcesConfig);
			mOutputStorageData = new IndustryStorageData(resourcesConfig);

			mIndustry = CreateIndustry(progressionConfig, resourcesConfig, recipe);

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

		private static IResourcesConfig CreateResourceConfig()
		{
			var configs = new Dictionary<short, IResourceConfig>
			{
				{ 1, new MockResourceConfig(1, "Wood", 3f) },
				{ 2, new MockResourceConfig(2, "Planks", 2.5f) },
				{ 3, new MockResourceConfig(3, "Coal", 2f) },
				{ 4, new MockResourceConfig(4, "Ore", 2f) },
				{ 5, new MockResourceConfig(5, "Metal", 3f) },
				{ 6, new MockResourceConfig(6, "Instruments", 1.5f) },
				{ 7, new MockResourceConfig(7, "Goods", 0.1f) },
			};
			var resourcesConfig = new MockResourcesConfig(configs);
			return resourcesConfig;
		}

		private static Recipe CreateRecipe()
		{
			return new Recipe(
				new[] { new ResourcePackage(1, 1) },
				new[] { new ResourcePackage(2, 2) });
		}

		private Industry CreateIndustry(
			IIndustryProgressionConfig progressionConfig,
			IResourcesConfig resourcesConfig,
			Recipe recipe
		)
		{
			return new Industry(
				mStateData,
				mProgressionData,
				mInputStorageData,
				mOutputStorageData,
				progressionConfig,
				resourcesConfig,
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
					new[] { new ResourcePackage(4, 5) },
					mCts.Token);
			});
			Assert.ThrowsAsync<InsufficientIndustryLevelException>(async () =>
			{
				await mIndustry.UnloadOutput(
					new[] { new ResourcePackage(4, 5) },
					mCts.Token);
			});
		}

		[Test]
		public void AnyQueryMethodExceptCanLevelUp_ReturnsFalse_IfLevel0()
		{
			Assert.IsFalse(mIndustry.CanLoadInput(new[] { new ResourcePackage(4, 5) }));
			Assert.IsFalse(mIndustry.CanUnloadOutput(new[] { new ResourcePackage(4, 5) }));
			Assert.IsFalse(mIndustry.CanProduce());
		}

		[Test]
		public async Task LoadInput_AddsToInputStorage()
		{
			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(4, 5) };
			await mIndustry.LoadInput(resources, mCts.Token);
			var inputStorageResources = mInputStorageData.Resources.ToArray();
			Assert.AreEqual(5, inputStorageResources[4 - 1].Amount);
		}

		[Test]
		public async Task LoadInput_TakesTime()
		{
			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(4, 5) };
			await mIndustry.LoadInput(resources, mCts.Token);
			AssertAreEqualDouble(2f * 1.5f * 5, Time.SecondsSinceStart);
		}

		[Test]
		public async Task LoadInput_Adds_UntilCancelled()
		{
			Time.ResetTimeProvider();
			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(7, 5) };

			await Task.WhenAll(
				mIndustry.LoadInput(resources, mCts.Token),
				CancelAfter(200));

			var inputStorageResources = mInputStorageData.Resources.ToArray();
			Assert.AreEqual(1, inputStorageResources[7 - 1].Amount);

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
			var resources = new[] { new ResourcePackage(7, 100) };

			await mIndustry.LoadInput(resources, mCts.Token);

			var inputStorageResources = mInputStorageData.Resources.ToArray();
			Assert.AreEqual(50, inputStorageResources[7 - 1].Amount);
		}

		[Test]
		public void CanLoadInput_ReturnsTrue_WhenInputIsEmpty()
		{
			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(4, 5) };
			Assert.IsTrue(mIndustry.CanLoadInput(resources));
		}

		[Test]
		public void CanLoadInput_ReturnsFalse_WhenInputWillBeOverCapacity()
		{
			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(4, 55) };
			Assert.IsFalse(mIndustry.CanLoadInput(resources));
		}

		[Test]
		public async Task UnloadOutput_RemovesFromOutputStorage_IfHasOutput()
		{
			mProgressionData.Level = 1;
			var resources1 = new[] { new ResourcePackage(4, 5) };
			var resources2 = new[] { new ResourcePackage(4, 3) };
			mOutputStorageData.AddResource(resources1[0]);

			await mIndustry.UnloadOutput(resources2, mCts.Token);
			var outputStorageResources = mOutputStorageData.Resources.ToArray();
			Assert.AreEqual(2, outputStorageResources[4 - 1].Amount);
		}

		[Test]
		public async Task UnloadOutput_TakesTime()
		{
			mProgressionData.Level = 1;
			var resources1 = new[] { new ResourcePackage(4, 5) };
			var resources2 = new[] { new ResourcePackage(4, 3) };
			mOutputStorageData.AddResource(resources1[0]);

			await mIndustry.UnloadOutput(resources2, mCts.Token);
			AssertAreEqualDouble(2f * 1.4f * 3, Time.SecondsSinceStart);
		}

		[Test]
		public async Task UnloadOutput_Removes_UntilCancelled()
		{
			Time.ResetTimeProvider();
			mProgressionData.Level = 1;
			var resources1 = new[] { new ResourcePackage(7, 5) };
			var resources2 = new[] { new ResourcePackage(7, 3) };
			mOutputStorageData.AddResource(resources1[0]);

			await Task.WhenAll(
				mIndustry.UnloadOutput(resources2, mCts.Token),
				CancelAfter(200));

			var outputStorageResources = mOutputStorageData.Resources.ToArray();
			Assert.AreEqual(4, outputStorageResources[7 - 1].Amount);

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
			var resources1 = new[] { new ResourcePackage(7, 5) };
			var resources2 = new[] { new ResourcePackage(7, 100) };
			mOutputStorageData.AddResource(resources1[0]);

			await mIndustry.UnloadOutput(resources2, mCts.Token);

			var outputStorageResources = mOutputStorageData.Resources.ToArray();
			Assert.AreEqual(0, outputStorageResources[7 - 1].Amount);
		}

		[Test]
		public async Task UnloadOutput_Removes_UntilAbleIfRemovedWhileWaiting()
		{
			Time.ResetTimeProvider();
			mProgressionData.Level = 1;
			var resources1 = new[] { new ResourcePackage(7, 5) };
			var resources2 = new[] { new ResourcePackage(7, 100) };
			mOutputStorageData.AddResource(resources1[0]);

			await Task.WhenAll(
				mIndustry.UnloadOutput(resources2, mCts.Token),
				RemoveAfter(200)
			);

			var outputStorageResources = mOutputStorageData.Resources.ToArray();
			Assert.AreEqual(0, outputStorageResources[7 - 1].Amount);

			async Task RemoveAfter(int milliseconds)
			{
				await Time.Delay(milliseconds);
				mOutputStorageData.RemoveResource(new ResourcePackage(7, 4));
			}
		}

		[Test]
		public void CanUnloadOutput_ReturnsFalse_WhenEmpty()
		{
			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(4, 5) };
			Assert.IsFalse(mIndustry.CanUnloadOutput(resources));
		}

		[Test]
		public void CanUnloadOutput_ReturnsTrue_WhenEnough()
		{
			mProgressionData.Level = 1;
			var resources1 = new[] { new ResourcePackage(4, 5) };
			var resources2 = new[] { new ResourcePackage(4, 3) };
			mOutputStorageData.AddResource(resources1[0]);
			Assert.IsTrue(mIndustry.CanUnloadOutput(resources2));
		}

		[Test]
		public void CanUnloadOutput_ReturnsFalse_WhenNotEnough()
		{
			mProgressionData.Level = 1;
			var resources1 = new[] { new ResourcePackage(4, 5) };
			var resources2 = new[] { new ResourcePackage(4, 8) };
			mOutputStorageData.AddResource(resources1[0]);
			Assert.IsFalse(mIndustry.CanUnloadOutput(resources2));
		}

		[Test]
		public async Task Produce_UsesInputAndStoresOutput()
		{
			mProgressionData.Level = 1;
			mInputStorageData.AddResource(new ResourcePackage(1, 5));

			await mIndustry.Produce(mCts.Token);

			Assert.AreEqual(10, mOutputStorageData.Resources.ToArray()[2 - 1].Amount);
		}

		[Test]
		public async Task Produce_TakesTime()
		{
			mProgressionData.Level = 1;
			mInputStorageData.AddResource(new ResourcePackage(1, 5));

			await mIndustry.Produce(mCts.Token);

			AssertAreEqualDouble(3f * 1.3f * 5 + 5 * 5 + 2.5f * 1.6f * 10, Time.SecondsSinceStart);
		}

		[Test]
		public async Task Produce_Produces_UntilCancelled()
		{
			var recipe = new Recipe(
				new[] { new ResourcePackage(7, 1) },
				new[] { new ResourcePackage(6, 2) }
			);
			mIndustry = CreateIndustry(
				CreateProgressionConfig(),
				CreateResourceConfig(),
				recipe);
			Time.ResetTimeProvider();
			mProgressionData.Level = 4;
			mInputStorageData.AddResource(new ResourcePackage(7, 5));

			await Task.WhenAll(
				mIndustry.Produce(mCts.Token),
				CancelAfter(3650)
			);

			Assert.AreEqual(5, mInputStorageData.Resources.ToArray()[7 - 1].Amount);
			Assert.AreEqual(0, mOutputStorageData.Resources.ToArray()[6 - 1].Amount);

			async Task CancelAfter(int milliseconds)
			{
				await Time.Delay(milliseconds);
				mCts.Cancel();
			}
		}

		[Test]
		public void CanProduce_ReturnsFalse_WhenInputEmpty()
		{
			mProgressionData.Level = 1;
			Assert.IsFalse(mIndustry.CanProduce());
		}

		[Test]
		public void CanProduce_ReturnsFalse_WhenNotEnoughInput()
		{
			var recipe = new Recipe(
				new[] { new ResourcePackage(7, 2) },
				new[] { new ResourcePackage(6, 2) }
			);
			mIndustry = CreateIndustry(
				CreateProgressionConfig(),
				CreateResourceConfig(),
				recipe);

			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(7, 1) };
			mInputStorageData.AddResource(resources[0]);
			Assert.IsFalse(mIndustry.CanProduce());
		}

		[Test]
		public void CanProduce_ReturnsFalse_WhenOutputWillBeOverCapacity()
		{
			mProgressionData.Level = 1;
			var resource = new ResourcePackage(2, 49);
			mOutputStorageData.AddResource(resource);
			Assert.IsFalse(mIndustry.CanProduce());
		}

		[Test]
		public void CanProduce_ReturnsFalse_IfStatusIsNotIdle()
		{
			mProgressionData.Level = 1;
			mInputStorageData.AddResource(new ResourcePackage(1, 5));
			mStateData.Status = IndustryStatus.Producing;
			Assert.IsFalse(mIndustry.CanProduce());
			mStateData.Status = IndustryStatus.LoadingInput;
			Assert.IsFalse(mIndustry.CanProduce());
			mStateData.Status = IndustryStatus.LoadingOutput;
			Assert.IsFalse(mIndustry.CanProduce());
			mStateData.Status = IndustryStatus.UnloadingInput;
			Assert.IsFalse(mIndustry.CanProduce());
			mStateData.Status = IndustryStatus.UnloadingOutput;
			Assert.IsFalse(mIndustry.CanProduce());
		}

		[Test]
		public void CanProduce_ReturnsTrue_WhenEnoughInputAndOutputWillNotBeOverCapacity()
		{
			mProgressionData.Level = 1;
			var inputResource = new ResourcePackage(1, 2);
			var outputResource = new ResourcePackage(2, 4);
			mInputStorageData.AddResource(inputResource);
			mOutputStorageData.AddResource(outputResource);
			Assert.IsTrue(mIndustry.CanProduce());
		}

		[Test]
		public async Task LoadInput_ChangesStateAndRevertsAfterFinished()
		{
			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(7, 100) };

			var statuses = new List<IndustryStatus>();
			mStateData.StatusChanged += status => statuses.Add(status);
			await mIndustry.LoadInput(resources, mCts.Token);

			Assert.AreEqual(2, statuses.Count);
			Assert.AreEqual(IndustryStatus.LoadingInput, statuses[0]);
			Assert.AreEqual(IndustryStatus.Idle, statuses[1]);
		}

		[Test]
		public async Task LoadInput_ChangesStateAndRevertsAfterCancelled()
		{
			Time.ResetTimeProvider();
			mProgressionData.Level = 1;
			var resources = new[] { new ResourcePackage(7, 5) };

			var statuses = new List<IndustryStatus>();
			mStateData.StatusChanged += status => statuses.Add(status);
			await Task.WhenAll(
				mIndustry.LoadInput(resources, mCts.Token),
				CancelAfter(200));

			Assert.AreEqual(2, statuses.Count);
			Assert.AreEqual(IndustryStatus.LoadingInput, statuses[0]);
			Assert.AreEqual(IndustryStatus.Idle, statuses[1]);

			async Task CancelAfter(int milliseconds)
			{
				await Time.Delay(milliseconds);
				mCts.Cancel();
			}
		}

		[Test]
		public async Task UnloadOutput_ChangesStateAndRevertsAfterFinished()
		{
			mProgressionData.Level = 1;
			var resources1 = new[] { new ResourcePackage(7, 5) };
			var resources2 = new[] { new ResourcePackage(7, 100) };
			mOutputStorageData.AddResource(resources1[0]);

			var statuses = new List<IndustryStatus>();
			mStateData.StatusChanged += status => statuses.Add(status);
			await mIndustry.UnloadOutput(resources2, mCts.Token);

			Assert.AreEqual(2, statuses.Count);
			Assert.AreEqual(IndustryStatus.UnloadingOutput, statuses[0]);
			Assert.AreEqual(IndustryStatus.Idle, statuses[1]);
		}

		[Test]
		public async Task UnloadOutput_ChangesStateAndRevertsAfterCancelled()
		{
			Time.ResetTimeProvider();
			mProgressionData.Level = 1;
			var resources1 = new[] { new ResourcePackage(7, 5) };
			var resources2 = new[] { new ResourcePackage(7, 3) };
			mOutputStorageData.AddResource(resources1[0]);

			var statuses = new List<IndustryStatus>();
			mStateData.StatusChanged += status => statuses.Add(status);
			await Task.WhenAll(
				mIndustry.UnloadOutput(resources2, mCts.Token),
				CancelAfter(200));

			Assert.AreEqual(2, statuses.Count);
			Assert.AreEqual(IndustryStatus.UnloadingOutput, statuses[0]);
			Assert.AreEqual(IndustryStatus.Idle, statuses[1]);

			async Task CancelAfter(int milliseconds)
			{
				await Time.Delay(milliseconds);
				mCts.Cancel();
			}
		}

		[Test]
		public async Task Produce_ChangesStateAndRevertsAfterFinished()
		{
			mProgressionData.Level = 1;
			mInputStorageData.AddResource(new ResourcePackage(1, 1));

			var statuses = new List<IndustryStatus>();
			mStateData.StatusChanged += status => statuses.Add(status);
			await mIndustry.Produce(mCts.Token);

			Assert.AreEqual(4, statuses.Count);
			Assert.AreEqual(IndustryStatus.UnloadingInput, statuses[0]);
			Assert.AreEqual(IndustryStatus.Producing, statuses[1]);
			Assert.AreEqual(IndustryStatus.LoadingOutput, statuses[2]);
			Assert.AreEqual(IndustryStatus.Idle, statuses[3]);
		}

		[Test]
		public async Task Produce_ChangesStateEachProductionCycle()
		{
			mProgressionData.Level = 1;
			mInputStorageData.AddResource(new ResourcePackage(1, 3));

			var statuses = new List<IndustryStatus>();
			mStateData.StatusChanged += status => statuses.Add(status);
			await mIndustry.Produce(mCts.Token);

			Assert.AreEqual(12, statuses.Count);
			Assert.AreEqual(IndustryStatus.UnloadingInput, statuses[0]);
			Assert.AreEqual(IndustryStatus.Producing, statuses[1]);
			Assert.AreEqual(IndustryStatus.LoadingOutput, statuses[2]);
			Assert.AreEqual(IndustryStatus.Idle, statuses[3]);
			Assert.AreEqual(IndustryStatus.UnloadingInput, statuses[4]);
			Assert.AreEqual(IndustryStatus.Producing, statuses[5]);
			Assert.AreEqual(IndustryStatus.LoadingOutput, statuses[6]);
			Assert.AreEqual(IndustryStatus.Idle, statuses[7]);
			Assert.AreEqual(IndustryStatus.UnloadingInput, statuses[8]);
			Assert.AreEqual(IndustryStatus.Producing, statuses[9]);
			Assert.AreEqual(IndustryStatus.LoadingOutput, statuses[10]);
			Assert.AreEqual(IndustryStatus.Idle, statuses[11]);
		}


		private static void AssertAreEqualDouble(double expected, double actual, double epsilon = 0.01f)
		{
			Assert.Greater(actual, expected - epsilon);
			Assert.Less(actual, expected + epsilon);
		}
	}
}