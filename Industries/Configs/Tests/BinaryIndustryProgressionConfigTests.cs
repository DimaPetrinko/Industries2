using System;
using Core.Exceptions;
using Industries.Configs.Implementation;
using NUnit.Framework;

namespace Industries.Configs.Tests
{
	internal class BinaryIndustryProgressionConfigTests
	{
		private IIndustryProgressionConfig mConfig;
		private IIndustryLevelConfig[] mConfigs;

		[SetUp]
		public void SetUp()
		{
			mConfigs = new[]
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
			mConfig = new BinaryIndustryProgressionConfig(mConfigs);
		}

		[Test]
		public void Constructor_Throws_IfProvidedConfigsCollectionIsNull()
		{
			IIndustryLevelConfig[] configs = null;
			Assert.Throws<ArgumentException>(() =>
			{
				var config = new BinaryIndustryProgressionConfig(configs);
			});
		}

		[Test]
		public void Constructor_Throws_IfMoreThan254ConfigsProvided()
		{
			var configs = new IIndustryLevelConfig[255];
			Assert.Throws<ArgumentException>(() =>
			{
				var config = new BinaryIndustryProgressionConfig(configs);
			});
		}

		[Test]
		public void Constructor_Throws_IfAnyConfigIsNull()
		{
			var configs = new IIndustryLevelConfig[10];
			for (var i = 0; i < configs.Length; i++)
			{
				if (i == 3)
					configs[i] = null;
				else
					configs[i] = new BinaryIndustryLevelConfig(0, 0, 0, 0, 0, 0, 0);
			}
			Assert.Throws<ArgumentException>(() =>
			{
				var config = new BinaryIndustryProgressionConfig(configs);
			});
		}

		[Test]
		public void GetConfigForLevel_ReturnsConfig()
		{
			var config = mConfig.GetConfigForLevel(1);
			Assert.IsNotNull(config);
			Assert.AreEqual(mConfigs[0], config);
		}

		[Test]
		public void GetConfigForLevel_Throws_IfZeroLevel()
		{
			var e = Assert.Throws<LevelConfigNotFoundException>(() => mConfig.GetConfigForLevel(0));
			Assert.AreEqual(0, e.Level);
		}

		[Test]
		public void GetConfigForLevel_Throws_IfNoSuchLevel()
		{
			Assert.Throws<LevelConfigNotFoundException>(() => mConfig.GetConfigForLevel(10));
		}

		[Test]
		public void MaxLevel_ReturnsCorrectValue()
		{
			Assert.AreEqual(4, mConfig.MaxLevel);
		}
	}
}