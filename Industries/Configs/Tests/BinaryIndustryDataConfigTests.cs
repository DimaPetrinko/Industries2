using System.Collections.Generic;
using Industries.Configs.Implementation;
using NUnit.Framework;

namespace Industries.Configs.Tests
{
	internal class BinaryIndustryDataConfigTests
	{
		private IIndustriesConfig mIndustriesConfig;

		[SetUp]
		public void SetUp()
		{
			var configs = new Dictionary<short, IIndustryDataConfig>
			{
				{ 1, new BinaryIndustryDataConfig("Logging camp", 1) },
				{ 2, new BinaryIndustryDataConfig("Coal mine", 2) },
				{ 3, new BinaryIndustryDataConfig("Iron mine", 3) },
				{ 4, new BinaryIndustryDataConfig("Lumber mill", 4) },
				{ 5, new BinaryIndustryDataConfig("Smelter", 5) },
				{ 6, new BinaryIndustryDataConfig("Smith", 6) },
				{ 7, new BinaryIndustryDataConfig("Factory", 7) },
			};

			mIndustriesConfig = new BinaryIndustriesConfig(configs);
		}

		[Test]
		public void Constructor_AssignsDictionary()
		{
			Assert.AreEqual(7, mIndustriesConfig.Industries.Count);

			var industryData = mIndustriesConfig.Industries[5];

			Assert.AreEqual("Smelter", industryData.Name);
			Assert.AreEqual(5, industryData.RecipeId);
		}

		[Test]
		public void Get_ThrowsKeyNotFoundException_IfId0()
		{
			Assert.Throws<KeyNotFoundException>(() =>
			{
				var industryData = mIndustriesConfig.Industries[0];
			});
		}

		[Test]
		public void Get_ThrowsKeyNotFoundException_IfNonExistentId()
		{
			Assert.Throws<KeyNotFoundException>(() =>
			{
				var industryData = mIndustriesConfig.Industries[99];
			});
		}
	}
}