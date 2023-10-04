using Industries.Data.Implementation;
using NUnit.Framework;

namespace Industries.Data.Tests
{
	internal class IndustryStateDataTests
	{
		private IIndustryStateMutableData mIndustryState;

		[SetUp]
		public void SetUp()
		{
			mIndustryState = new IndustryStateData(1);
		}

		[Test]
		public void Constructor_SetsDefaultValues()
		{
			var industryState = new IndustryStateData(1);

			Assert.AreEqual(1, industryState.Id);
			Assert.AreEqual(IndustryStatus.Idle, industryState.Status);
		}

		[Test]
		public void SetStatus_ChangesStatus()
		{
			mIndustryState.Status = IndustryStatus.Producing;

			Assert.AreEqual(IndustryStatus.Producing, mIndustryState.Status);
		}

		[Test]
		public void SetStatus_IfDifferentValue_TriggersEvent()
		{
			var triggered = false;

			mIndustryState.StatusChanged += OnStatusChanged;
			mIndustryState.Status = IndustryStatus.LoadingInput;

			void OnStatusChanged(IndustryStatus status)
			{
				triggered = true;
			}

			Assert.IsTrue(triggered);
		}

		[Test]
		public void SetStatus_IfSameValue_DoesNotTriggerEvent()
		{
			var triggered = false;

			mIndustryState.Status = IndustryStatus.LoadingInput;
			mIndustryState.StatusChanged += OnStatusChanged;
			mIndustryState.Status = IndustryStatus.LoadingInput;

			void OnStatusChanged(IndustryStatus status)
			{
				triggered = true;
			}

			Assert.IsFalse(triggered);
		}

		[Test]
		public void SetStatus_TriggersEventWithCorrectValue()
		{
			var receivedStatus = IndustryStatus.Idle;

			mIndustryState.StatusChanged += OnStatusChanged;
			mIndustryState.Status = IndustryStatus.LoadingInput;

			void OnStatusChanged(IndustryStatus status)
			{
				receivedStatus = status;
			}

			Assert.AreEqual(IndustryStatus.LoadingInput, receivedStatus);
		}

		[Test]
		public void SetStatus_WhenEventIsTriggered_HasCorrectValue()
		{
			mIndustryState.StatusChanged += OnStatusChanged;
			mIndustryState.Status = IndustryStatus.LoadingInput;

			void OnStatusChanged(IndustryStatus status)
			{
				Assert.AreEqual(IndustryStatus.LoadingInput, mIndustryState.Status);
			}
		}
	}
}