using System;
using System.Collections.Generic;
using System.Linq;
using Industries.Data.Implementation;
using Industries.Exceptions;
using NUnit.Framework;
using Resources;
using Resources.Configs;
using Resources.Exceptions;

namespace Industries.Data.Tests
{
	internal class IndustryStorageDataTests
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
				if (AllResourceConfigs.TryGetValue(id, out var config))
				{
					return config;
				}

				throw new UnknownResourceException(id);
			}
		}

		#endregion

		private IIndustryStorageMutableData mData;
		private IResourcesConfig mResourcesConfig;

		[SetUp]
		public void SetUp()
		{
			mResourcesConfig = CreateResourcesConfig();
			mData = new IndustryStorageData(mResourcesConfig);
			mData.CurrentCapacity = 10;
		}

		private static IResourcesConfig CreateResourcesConfig()
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

		[Test]
		public void Constructor_CreatesWithDefaultValues()
		{
			var data = new IndustryStorageData(mResourcesConfig);

			Assert.AreEqual(0, data.CurrentAmount);
			Assert.AreEqual(0, data.CurrentCapacity);
			Assert.IsNotEmpty(data.Resources);
			Assert.AreEqual(mResourcesConfig.AllResourceConfigs.Count, data.Resources.Count());
		}

		[Test]
		public void SetCapacity_ChangesCapacity()
		{
			var data = new IndustryStorageData(mResourcesConfig);
			data.CurrentCapacity = 10;

			Assert.AreEqual(10, data.CurrentCapacity);
		}

		[Test]
		public void CanAdd_ReturnsTrue_WhenNoneAdded()
		{
			var resource = new ResourcePackage(2, 2);
			Assert.IsTrue(mData.CanAddResources(new[] { resource }));
		}

		[Test]
		public void CanAdd_ReturnsFalse_WhenExceedingCapacity()
		{
			var resource = new ResourcePackage(2, 12);
			Assert.IsFalse(mData.CanAddResources(new[] { resource }));
		}

		[Test]
		public void CanAdd_ReturnsFalse_WhenAtCapacity()
		{
			var resource1 = new ResourcePackage(1, 10);
			var resource2 = new ResourcePackage(2, 1);
			mData.AddResource(resource1);
			Assert.IsFalse(mData.CanAddResources(new[] { resource2 }));
		}

		[Test]
		public void CanAdd_ReturnsFalse_WhenAddingNone()
		{
			var resource = new ResourcePackage(0, 2);
			Assert.IsFalse(mData.CanAddResources(new[] { resource }));
		}

		[Test]
		public void CanAdd_ReturnsTrue_WhenAdding0()
		{
			var resource = new ResourcePackage(1, 0);
			Assert.IsTrue(mData.CanAddResources(new[] { resource }));
		}

		[Test]
		public void CanRemove_ReturnsFalse_WhenNoneAdded()
		{
			var resource = new ResourcePackage(1, 5);
			Assert.IsFalse(mData.CanRemoveResources(new[] { resource }));
		}

		[Test]
		public void CanRemove_ReturnsFalse_WhenRemovingNone()
		{
			var resource1 = new ResourcePackage(2, 4);
			var resource2 = new ResourcePackage(0, 1);
			mData.AddResource(resource1);
			Assert.IsFalse(mData.CanRemoveResources(new[] { resource2 }));
		}

		[Test]
		public void CanRemove_ReturnsTrue_WhenRemoving0()
		{
			var resource1 = new ResourcePackage(2, 4);
			var resource2 = new ResourcePackage(2, 0);
			mData.AddResource(resource1);
			Assert.IsTrue(mData.CanRemoveResources(new[] { resource2 }));
		}

		[Test]
		public void CanRemove_ReturnsFalse_WhenRemovingMoreThanStored()
		{
			var resource1 = new ResourcePackage(2, 4);
			var resource2 = new ResourcePackage(2, 5);
			mData.AddResource(resource1);
			Assert.IsFalse(mData.CanRemoveResources(new[] { resource2 }));
		}

		[Test]
		public void CanRemove_ReturnsTrue_WhenRemovingLessThanStored()
		{
			var resource1 = new ResourcePackage(2, 4);
			var resource2 = new ResourcePackage(2, 3);
			mData.AddResource(resource1);
			Assert.IsTrue(mData.CanRemoveResources(new[] { resource2 }));
		}

		[Test]
		public void CanRemove_ReturnsFalse_WhenRemovingResourceThatIsNotStored()
		{
			var resource1 = new ResourcePackage(2, 4);
			var resource2 = new ResourcePackage(1, 3);
			mData.AddResource(resource1);
			Assert.IsFalse(mData.CanRemoveResources(new[] { resource2 }));
		}

		[Test]
		public void Add_AddsResource_IfNoneAdded()
		{
			var resource = new ResourcePackage(2, 4);
			mData.AddResource(resource);
			Assert.AreEqual(4, mData.Resources.ToArray()[2 - 1].Amount);
			Assert.AreEqual(4, mData.CurrentAmount);
		}

		[Test]
		public void Add_AddsResource_IfOtherAdded()
		{
			var resource1 = new ResourcePackage(1, 1);
			var resource2 = new ResourcePackage(2, 4);
			mData.AddResource(resource1);
			mData.AddResource(resource2);
			Assert.AreEqual(4, mData.Resources.ToArray()[2 - 1].Amount);
			Assert.AreEqual(5, mData.CurrentAmount);
		}

		[Test]
		public void Add_AddsResource_IfSameAddedAndNotAtCapacity()
		{
			var resource1 = new ResourcePackage(2, 1);
			var resource2 = new ResourcePackage(2, 4);
			mData.AddResource(resource1);
			mData.AddResource(resource2);
			Assert.AreEqual(5, mData.Resources.ToArray()[2 - 1].Amount);
			Assert.AreEqual(5, mData.CurrentAmount);
		}

		[Test]
		public void Add_AddsResource_IfMeetsCapacity()
		{
			var resource1 = new ResourcePackage(1, 7);
			var resource2 = new ResourcePackage(2, 3);
			mData.AddResource(resource1);
			mData.AddResource(resource2);
			Assert.AreEqual(3, mData.Resources.ToArray()[2 - 1].Amount);
			Assert.AreEqual(10, mData.CurrentAmount);

		}

		[Test]
		public void Add_AddsResource_IfAdding0()
		{
			var resource = new ResourcePackage(2, 0);
			mData.AddResource(resource);
			Assert.AreEqual(0, mData.Resources.ToArray()[2 - 1].Amount);
			Assert.AreEqual(0, mData.CurrentAmount);
		}

		[Test]
		public void Add_Throws_IfAlreadyAtCapacity()
		{
			var resource1 = new ResourcePackage(1, 10);
			var resource2 = new ResourcePackage(2, 1);
			mData.AddResource(resource1);
			Assert.Throws<IndustryStorageAdditionException>(() => mData.AddResource(resource2));
			Assert.AreEqual(10, mData.CurrentAmount);
		}

		[Test]
		public void Add_Throws_IfAddingNone()
		{
			var resource = new ResourcePackage(0, 1);
			Assert.Throws<IndustryStorageAdditionException>(() => mData.AddResource(resource));
			Assert.AreEqual(0, mData.CurrentAmount);
		}

		[Test]
		public void Add_Throws_IfWillExceedCapacityWhenAddingSingleType()
		{
			var resource = new ResourcePackage(2, 11);
			Assert.Throws<IndustryStorageAdditionException>(() => mData.AddResource(resource));
			Assert.AreEqual(0, mData.CurrentAmount);
		}

		[Test]
		public void Add_Throws_IfWillExceedCapacityWhenAddingMultipleType()
		{
			var resource1 = new ResourcePackage(2, 6);
			var resource2 = new ResourcePackage(2, 5);
			mData.AddResource(resource1);
			Assert.Throws<IndustryStorageAdditionException>(() => mData.AddResource(resource2));
			Assert.AreEqual(6, mData.CurrentAmount);
		}

		[Test]
		public void Remove_RemovesResource_IfAdded()
		{
			var resource = new ResourcePackage(2, 5);
			mData.AddResource(resource);
			mData.RemoveResource(resource);
			Assert.AreEqual(0, mData.CurrentAmount);
		}

		[Test]
		public void Remove_RemovesResource_IfAddedMore()
		{
			var resource1 = new ResourcePackage(2, 7);
			var resource2 = new ResourcePackage(2, 5);
			mData.AddResource(resource1);
			mData.RemoveResource(resource2);
			Assert.AreEqual(2, mData.CurrentAmount);
		}

		[Test]
		public void Remove_RemovesResource_IfAddedMultiple()
		{
			var resource1 = new ResourcePackage(1, 3);
			var resource2 = new ResourcePackage(2, 4);
			var resource3 = new ResourcePackage(2, 2);
			mData.AddResource(resource1);
			mData.AddResource(resource2);
			mData.RemoveResource(resource3);
			Assert.AreEqual(5, mData.CurrentAmount);
		}

		[Test]
		public void Remove_RemovesResource_IfRemoving0()
		{
			var resource1 = new ResourcePackage(2, 4);
			var resource2 = new ResourcePackage(2, 0);
			mData.AddResource(resource1);
			mData.RemoveResource(resource2);
			Assert.AreEqual(4, mData.CurrentAmount);
		}

		[Test]
		public void Remove_Throws_IfNoneAdded()
		{
			var resource = new ResourcePackage(2, 2);
			Assert.Throws<IndustryStorageRemovalException>(() => mData.RemoveResource(resource));
			Assert.AreEqual(0, mData.CurrentAmount);
		}

		[Test]
		public void Remove_Throws_IfOtherAdded()
		{
			var resource1 = new ResourcePackage(1, 3);
			var resource2 = new ResourcePackage(2, 2);
			mData.AddResource(resource1);
			Assert.Throws<IndustryStorageRemovalException>(() => mData.RemoveResource(resource2));
			Assert.AreEqual(3, mData.CurrentAmount);
		}

		[Test]
		public void Remove_Throws_IfLessAdded()
		{
			var resource1 = new ResourcePackage(2, 3);
			var resource2 = new ResourcePackage(2, 5);
			mData.AddResource(resource1);
			Assert.Throws<IndustryStorageRemovalException>(() => mData.RemoveResource(resource2));
			Assert.AreEqual(3, mData.CurrentAmount);
		}

		[Test]
		public void Remove_Throws_IfRemovingNone()
		{
			var resource1 = new ResourcePackage(2, 3);
			var resource2 = new ResourcePackage(0, 3);
			mData.AddResource(resource1);
			var e = Assert.Throws<IndustryStorageRemovalException>(() => mData.RemoveResource(resource2));
			Assert.AreEqual(3, mData.CurrentAmount);
			Assert.AreEqual(resource2, e.Resources.ToArray()[0]);
		}

		[Test]
		public void Add_TriggersEvent()
		{
			var triggered = false;
			var resource = new ResourcePackage(2, 4);

			mData.ResourceAdded += receivedResource => triggered = true;
			mData.AddResource(resource);

			Assert.IsTrue(triggered);
		}

		[Test]
		public void Add_TriggersEvent_WithCorrectValue()
		{
			var receivedResource = new ResourcePackage();
			var resource = new ResourcePackage(2, 4);

			mData.ResourceAdded += r => receivedResource = r;
			mData.AddResource(resource);

			Assert.AreEqual(resource, receivedResource);
		}

		[Test]
		public void Add_WhenThrows_DoesNotTriggerEvent()
		{
			var triggered = false;
			var resource = new ResourcePackage(0, 4);

			mData.ResourceAdded += receivedResource => triggered = true;
			try
			{
				mData.AddResource(resource);
			}
			catch (Exception)
			{
			}

			Assert.IsFalse(triggered);
		}

		[Test]
		public void Remove_TriggersEvent()
		{
			var triggered = false;
			var resource1 = new ResourcePackage(2, 8);
			var resource2 = new ResourcePackage(2, 4);

			mData.AddResource(resource1);
			mData.ResourceRemoved += receivedResource => triggered = true;
			mData.RemoveResource(resource2);

			Assert.IsTrue(triggered);
		}

		[Test]
		public void Remove_TriggersEvent_WithCorrectValue()
		{
			var receivedResource = new ResourcePackage();
			var resource1 = new ResourcePackage(2, 8);
			var resource2 = new ResourcePackage(2, 4);

			mData.AddResource(resource1);
			mData.ResourceRemoved += r => receivedResource = r;
			mData.RemoveResource(resource2);

			Assert.AreEqual(resource2, receivedResource);
		}

		[Test]
		public void Remove_WhenThrows_DoesNotTriggerEvent()
		{
			var triggered = false;
			var resource = new ResourcePackage(0, 4);

			mData.ResourceRemoved += receivedResource => triggered = true;
			try
			{
				mData.RemoveResource(resource);
			}
			catch (Exception)
			{
			}

			Assert.IsFalse(triggered);
		}
	}
}