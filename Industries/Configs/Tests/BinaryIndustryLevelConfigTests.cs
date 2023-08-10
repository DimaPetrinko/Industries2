using Industries.Configs.Implementation;
using NUnit.Framework;

namespace Industries.Configs.Tests
{
	internal class BinaryIndustryLevelConfigTests
	{
		private IIndustryLevelConfig mConfig;

		[SetUp]
		public void SetUp()
		{
			mConfig = new BinaryIndustryLevelConfig(
				5f,
				1.5f,
				1.3f,
				1.6f,
				1.4f,
				50,
				50
			);
		}

		[Test]
		public void Constructor_SetsValues()
		{
			Assert.AreEqual(5f, mConfig.ProductionTime);
			Assert.AreEqual(1.5f, mConfig.InputLoadingMultiplier);
			Assert.AreEqual(1.3f, mConfig.InputUnloadingMultiplier);
			Assert.AreEqual(1.6f, mConfig.OutputLoadingMultiplier);
			Assert.AreEqual(1.4f, mConfig.OutputUnloadingMultiplier);
			Assert.AreEqual(50, mConfig.InputAreaCapacity);
			Assert.AreEqual(50, mConfig.OutputAreaCapacity);
		}
	}
}