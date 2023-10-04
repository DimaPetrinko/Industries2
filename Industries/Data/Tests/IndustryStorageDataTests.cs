using System;
using System.Linq;
using Industries.Data.Implementation;
using Industries.Exceptions;
using Items;
using NUnit.Framework;
using Resources;

namespace Industries.Data.Tests
{
	internal class IndustryStorageDataTests
	{
		private IIndustryStorageMutableData mData;

		[SetUp]
		public void SetUp()
		{
			mData = new IndustryStorageData();
			mData.CurrentCapacity = 10;
		}

		[Test]
		public void Constructor_CreatesWithDefaultValues()
		{
			var data = new IndustryStorageData();

			Assert.AreEqual(0, data.CurrentAmount);
			Assert.AreEqual(0, data.CurrentCapacity);
			Assert.IsNotEmpty(data.Resources);
			Assert.AreEqual(Enum.GetValues(typeof(ItemType)).Length - 1, data.Resources.Count());
		}

		[Test]
		public void SetCapacity_ChangesCapacity()
		{
			var data = new IndustryStorageData();
			data.CurrentCapacity = 10;

			Assert.AreEqual(10, data.CurrentCapacity);
		}

		[Test]
		public void CanAdd_ReturnsTrue_WhenNoneAdded()
		{
			var resource = new ResourcePackage(ItemType.Planks, 2);
			Assert.IsTrue(mData.CanAddResources(new[] { resource }));
		}

		[Test]
		public void CanAdd_ReturnsFalse_WhenExceedingCapacity()
		{
			var resource = new ResourcePackage(ItemType.Planks, 12);
			Assert.IsFalse(mData.CanAddResources(new[] { resource }));
		}

		[Test]
		public void CanAdd_ReturnsFalse_WhenAtCapacity()
		{
			var resource1 = new ResourcePackage(ItemType.Wood, 10);
			var resource2 = new ResourcePackage(ItemType.Planks, 1);
			mData.AddResource(resource1);
			Assert.IsFalse(mData.CanAddResources(new[] { resource2 }));
		}

		[Test]
		public void CanAdd_ReturnsFalse_WhenAddingNone()
		{
			var resource = new ResourcePackage(ItemType.None, 2);
			Assert.IsFalse(mData.CanAddResources(new[] { resource }));
		}

		[Test]
		public void CanAdd_ReturnsTrue_WhenAdding0()
		{
			var resource = new ResourcePackage(ItemType.Wood, 0);
			Assert.IsTrue(mData.CanAddResources(new[] { resource }));
		}

		[Test]
		public void CanRemove_ReturnsFalse_WhenNoneAdded()
		{
			var resource = new ResourcePackage(ItemType.Wood, 5);
			Assert.IsFalse(mData.CanRemoveResources(new[] { resource }));
		}

		[Test]
		public void CanRemove_ReturnsFalse_WhenRemovingNone()
		{
			var resource1 = new ResourcePackage(ItemType.Planks, 4);
			var resource2 = new ResourcePackage(ItemType.None, 1);
			mData.AddResource(resource1);
			Assert.IsFalse(mData.CanRemoveResources(new[] { resource2 }));
		}

		[Test]
		public void CanRemove_ReturnsTrue_WhenRemoving0()
		{
			var resource1 = new ResourcePackage(ItemType.Planks, 4);
			var resource2 = new ResourcePackage(ItemType.Planks, 0);
			mData.AddResource(resource1);
			Assert.IsTrue(mData.CanRemoveResources(new[] { resource2 }));
		}

		[Test]
		public void CanRemove_ReturnsFalse_WhenRemovingMoreThanStored()
		{
			var resource1 = new ResourcePackage(ItemType.Planks, 4);
			var resource2 = new ResourcePackage(ItemType.Planks, 5);
			mData.AddResource(resource1);
			Assert.IsFalse(mData.CanRemoveResources(new[] { resource2 }));
		}

		[Test]
		public void CanRemove_ReturnsTrue_WhenRemovingLessThanStored()
		{
			var resource1 = new ResourcePackage(ItemType.Planks, 4);
			var resource2 = new ResourcePackage(ItemType.Planks, 3);
			mData.AddResource(resource1);
			Assert.IsTrue(mData.CanRemoveResources(new[] { resource2 }));
		}

		[Test]
		public void CanRemove_ReturnsFalse_WhenRemovingResourceThatIsNotStored()
		{
			var resource1 = new ResourcePackage(ItemType.Planks, 4);
			var resource2 = new ResourcePackage(ItemType.Wood, 3);
			mData.AddResource(resource1);
			Assert.IsFalse(mData.CanRemoveResources(new[] { resource2 }));
		}

		[Test]
		public void Add_AddsResource_IfNoneAdded()
		{
			var resource = new ResourcePackage(ItemType.Planks, 4);
			mData.AddResource(resource);
			Assert.AreEqual(4, mData.Resources.ToArray()[(int)ItemType.Planks - 1].Amount);
			Assert.AreEqual(4, mData.CurrentAmount);
		}

		[Test]
		public void Add_AddsResource_IfOtherAdded()
		{
			var resource1 = new ResourcePackage(ItemType.Wood, 1);
			var resource2 = new ResourcePackage(ItemType.Planks, 4);
			mData.AddResource(resource1);
			mData.AddResource(resource2);
			Assert.AreEqual(4, mData.Resources.ToArray()[(int)ItemType.Planks - 1].Amount);
			Assert.AreEqual(5, mData.CurrentAmount);
		}

		[Test]
		public void Add_AddsResource_IfSameAddedAndNotAtCapacity()
		{
			var resource1 = new ResourcePackage(ItemType.Planks, 1);
			var resource2 = new ResourcePackage(ItemType.Planks, 4);
			mData.AddResource(resource1);
			mData.AddResource(resource2);
			Assert.AreEqual(5, mData.Resources.ToArray()[(int)ItemType.Planks - 1].Amount);
			Assert.AreEqual(5, mData.CurrentAmount);
		}

		[Test]
		public void Add_AddsResource_IfMeetsCapacity()
		{
			var resource1 = new ResourcePackage(ItemType.Wood, 7);
			var resource2 = new ResourcePackage(ItemType.Planks, 3);
			mData.AddResource(resource1);
			mData.AddResource(resource2);
			Assert.AreEqual(3, mData.Resources.ToArray()[(int)ItemType.Planks - 1].Amount);
			Assert.AreEqual(10, mData.CurrentAmount);

		}

		[Test]
		public void Add_AddsResource_IfAdding0()
		{
			var resource = new ResourcePackage(ItemType.Planks, 0);
			mData.AddResource(resource);
			Assert.AreEqual(0, mData.Resources.ToArray()[(int)ItemType.Planks - 1].Amount);
			Assert.AreEqual(0, mData.CurrentAmount);
		}

		[Test]
		public void Add_Throws_IfAlreadyAtCapacity()
		{
			var resource1 = new ResourcePackage(ItemType.Wood, 10);
			var resource2 = new ResourcePackage(ItemType.Planks, 1);
			mData.AddResource(resource1);
			Assert.Throws<IndustryStorageAdditionException>(() => mData.AddResource(resource2));
			Assert.AreEqual(10, mData.CurrentAmount);
		}

		[Test]
		public void Add_Throws_IfAddingNone()
		{
			var resource = new ResourcePackage(ItemType.None, 1);
			Assert.Throws<IndustryStorageAdditionException>(() => mData.AddResource(resource));
			Assert.AreEqual(0, mData.CurrentAmount);
		}

		[Test]
		public void Add_Throws_IfWillExceedCapacityWhenAddingSingleType()
		{
			var resource = new ResourcePackage(ItemType.Planks, 11);
			Assert.Throws<IndustryStorageAdditionException>(() => mData.AddResource(resource));
			Assert.AreEqual(0, mData.CurrentAmount);
		}

		[Test]
		public void Add_Throws_IfWillExceedCapacityWhenAddingMultipleType()
		{
			var resource1 = new ResourcePackage(ItemType.Planks, 6);
			var resource2 = new ResourcePackage(ItemType.Planks, 5);
			mData.AddResource(resource1);
			Assert.Throws<IndustryStorageAdditionException>(() => mData.AddResource(resource2));
			Assert.AreEqual(6, mData.CurrentAmount);
		}

		[Test]
		public void Remove_RemovesResource_IfAdded()
		{
			var resource = new ResourcePackage(ItemType.Planks, 5);
			mData.AddResource(resource);
			mData.RemoveResource(resource);
			Assert.AreEqual(0, mData.CurrentAmount);
		}

		[Test]
		public void Remove_RemovesResource_IfAddedMore()
		{
			var resource1 = new ResourcePackage(ItemType.Planks, 7);
			var resource2 = new ResourcePackage(ItemType.Planks, 5);
			mData.AddResource(resource1);
			mData.RemoveResource(resource2);
			Assert.AreEqual(2, mData.CurrentAmount);
		}

		[Test]
		public void Remove_RemovesResource_IfAddedMultiple()
		{
			var resource1 = new ResourcePackage(ItemType.Wood, 3);
			var resource2 = new ResourcePackage(ItemType.Planks, 4);
			var resource3 = new ResourcePackage(ItemType.Planks, 2);
			mData.AddResource(resource1);
			mData.AddResource(resource2);
			mData.RemoveResource(resource3);
			Assert.AreEqual(5, mData.CurrentAmount);
		}

		[Test]
		public void Remove_RemovesResource_IfRemoving0()
		{
			var resource1 = new ResourcePackage(ItemType.Planks, 4);
			var resource2 = new ResourcePackage(ItemType.Planks, 0);
			mData.AddResource(resource1);
			mData.RemoveResource(resource2);
			Assert.AreEqual(4, mData.CurrentAmount);
		}

		[Test]
		public void Remove_Throws_IfNoneAdded()
		{
			var resource = new ResourcePackage(ItemType.Planks, 2);
			Assert.Throws<IndustryStorageRemovalException>(() => mData.RemoveResource(resource));
			Assert.AreEqual(0, mData.CurrentAmount);
		}

		[Test]
		public void Remove_Throws_IfOtherAdded()
		{
			var resource1 = new ResourcePackage(ItemType.Wood, 3);
			var resource2 = new ResourcePackage(ItemType.Planks, 2);
			mData.AddResource(resource1);
			Assert.Throws<IndustryStorageRemovalException>(() => mData.RemoveResource(resource2));
			Assert.AreEqual(3, mData.CurrentAmount);
		}

		[Test]
		public void Remove_Throws_IfLessAdded()
		{
			var resource1 = new ResourcePackage(ItemType.Planks, 3);
			var resource2 = new ResourcePackage(ItemType.Planks, 5);
			mData.AddResource(resource1);
			Assert.Throws<IndustryStorageRemovalException>(() => mData.RemoveResource(resource2));
			Assert.AreEqual(3, mData.CurrentAmount);
		}

		[Test]
		public void Remove_Throws_IfRemovingNone()
		{
			var resource1 = new ResourcePackage(ItemType.Planks, 3);
			var resource2 = new ResourcePackage(ItemType.None, 3);
			mData.AddResource(resource1);
			var e = Assert.Throws<IndustryStorageRemovalException>(() => mData.RemoveResource(resource2));
			Assert.AreEqual(3, mData.CurrentAmount);
			Assert.AreEqual(resource2, e.Resources.ToArray()[0]);
		}

		[Test]
		public void Add_TriggersEvent()
		{
			var triggered = false;
			var resource = new ResourcePackage(ItemType.Planks, 4);

			mData.ResourceAdded += receivedResource => triggered = true;
			mData.AddResource(resource);

			Assert.IsTrue(triggered);
		}

		[Test]
		public void Add_TriggersEvent_WithCorrectValue()
		{
			var receivedResource = new ResourcePackage();
			var resource = new ResourcePackage(ItemType.Planks, 4);

			mData.ResourceAdded += r => receivedResource = r;
			mData.AddResource(resource);

			Assert.AreEqual(resource, receivedResource);
		}

		[Test]
		public void Add_WhenThrows_DoesNotTriggerEvent()
		{
			var triggered = false;
			var resource = new ResourcePackage(ItemType.None, 4);

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
			var resource1 = new ResourcePackage(ItemType.Planks, 8);
			var resource2 = new ResourcePackage(ItemType.Planks, 4);

			mData.AddResource(resource1);
			mData.ResourceRemoved += receivedResource => triggered = true;
			mData.RemoveResource(resource2);

			Assert.IsTrue(triggered);
		}

		[Test]
		public void Remove_TriggersEvent_WithCorrectValue()
		{
			var receivedResource = new ResourcePackage();
			var resource1 = new ResourcePackage(ItemType.Planks, 8);
			var resource2 = new ResourcePackage(ItemType.Planks, 4);

			mData.AddResource(resource1);
			mData.ResourceRemoved += r => receivedResource = r;
			mData.RemoveResource(resource2);

			Assert.AreEqual(resource2, receivedResource);
		}

		[Test]
		public void Remove_WhenThrows_DoesNotTriggerEvent()
		{
			var triggered = false;
			var resource = new ResourcePackage(ItemType.None, 4);

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