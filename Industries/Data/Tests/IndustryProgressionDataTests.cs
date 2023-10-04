using Industries.Data.Implementation;
using NUnit.Framework;

namespace Industries.Data.Tests
{
	internal class IndustryProgressionDataTests
	{
		private IIndustryProgressionMutableData mData;

		[SetUp]
		public void SetUp()
		{
			mData = new IndustryProgressionData();
		}

		[Test]
		public void Constructor_SetsDefaultValues()
		{
			Assert.AreEqual(0, mData.Level);
		}

		[Test]
		public void SetLevel_SetsValue()
		{
			mData.Level = 5;
			Assert.AreEqual(5, mData.Level);
		}

		[Test]
		public void SetLevel_IfDifferent_TriggersEvent()
		{
			var triggered = false;
			mData.LevelChanged += level => triggered = true;
			mData.Level = 5;

			Assert.IsTrue(triggered);
		}

		[Test]
		public void SetLevel_IfSame_DoesNotTriggerEvent()
		{
			var triggered = false;
			mData.Level = 5;
			mData.LevelChanged += level => triggered = true;
			mData.Level = 5;

			Assert.IsFalse(triggered);
		}

		[Test]
		public void SetLevel_TriggersEventWithCorrectValue()
		{
			var receivedLevel = 0;

			mData.LevelChanged += level => receivedLevel = level;
			mData.Level = 5;

			Assert.AreEqual(5, receivedLevel);
		}

		[Test]
		public void SetLevel_WhenEventIsTriggered_HasCorrectValue()
		{
			mData.LevelChanged += OnLevelChanged;
			mData.Level = 5;

			void OnLevelChanged(byte level)
			{
				Assert.AreEqual(5, mData.Level);
			}
		}
	}
}