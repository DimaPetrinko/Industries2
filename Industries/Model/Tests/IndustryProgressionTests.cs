using Industries.Configs;
using Industries.Configs.Implementation;
using Industries.Data;
using Industries.Data.Implementation;
using Industries.Exceptions;
using Industries.Model.Implementation;
using NUnit.Framework;

namespace Industries.Model.Tests
{
	public class IndustryProgressionTests
	{
		private IIndustryProgressionMutableData mData;
		private IIndustryProgression mProgression;

		[SetUp]
		public void SetUp()
		{
			mData = new IndustryProgressionData();
			mProgression = new IndustryProgression(mData, CreateProgressionConfig());
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

		[Test]
		public void LevelUp_LevelIs0_ByDefault()
		{
			Assert.AreEqual(0, mData.Level);
		}

		[Test]
		public void LevelUp_IncrementsLevelBy1()
		{
			mProgression.LevelUp();
			Assert.AreEqual(1, mData.Level);
		}

		[Test]
		public void LevelUp_Throws_IfAtMaxLevel()
		{
			mData.Level = 4;

			var e = Assert.Throws<MaxIndustryLevelReachedException>(() => mProgression.LevelUp());
			Assert.AreEqual(5, e.Level);
			Assert.AreEqual(4, mData.Level);
		}

		[Test]
		public void CanLevelUp_ReturnsTrue_IfLevel0()
		{
			Assert.IsTrue(mProgression.CanLevelUp());
		}

		[Test]
		public void CanLevelUp_ReturnsTrue_IfLevelLessThanMax()
		{
			mData.Level = 3;
			Assert.IsTrue(mProgression.CanLevelUp());
		}

		[Test]
		public void CanLevelUp_ReturnsFalse_IfAtMaxLevel()
		{
			mData.Level = 4;
			Assert.IsFalse(mProgression.CanLevelUp());
		}
	}
}