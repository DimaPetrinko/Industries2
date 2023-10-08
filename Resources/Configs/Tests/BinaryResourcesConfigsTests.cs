using System;
using System.Linq;
using NUnit.Framework;
using Resources.Configs.Implementation;
using Resources.Exceptions;

namespace Resources.Configs.Tests
{
	internal class BinaryResourcesConfigsTests
	{
		private BinaryResourcesConfig mConfig;

		[SetUp]
		public void SetUp()
		{
			var configs = new[]
			{
				new BinaryResourceConfig(1, "Wood", 3f),
				new BinaryResourceConfig(2, "Planks", 2f),
				new BinaryResourceConfig(3, "Coal", 4f),
				new BinaryResourceConfig(4, "Ore", 4f),
				new BinaryResourceConfig(5, "Metal", 2f),
				new BinaryResourceConfig(6, "Instruments", 1.5f),
				new BinaryResourceConfig(7, "Goods", 1.5f),
			};

			mConfig = new BinaryResourcesConfig(configs);
		}

		[Test]
		public void Constructor_Throws_WhenConfigsWithSameIdProvided()
		{
			var configs = new[]
			{
				new BinaryResourceConfig(2, "Resource1", 1f),
				new BinaryResourceConfig(2, "Resource2", 2f),
			};
			var e = Assert.Throws<DuplicateResourceIdsException>(() =>
			{
				var resourcesConfig = new BinaryResourcesConfig(configs);
			});

			Assert.AreEqual(2, e.Id);
		}

		[Test]
		public void Constructor_Throws_WhenNullProvided()
		{
			var configs = new[]
			{
				new BinaryResourceConfig(2, "Resource1", 1f),
				null
			};

			Assert.Throws<ArgumentException>(() =>
			{
				var resourcesConfig = new BinaryResourcesConfig(configs);
			});
		}

		[Test]
		public void GetAll_ReturnsCorrectConfigs()
		{
			Assert.IsNotNull(mConfig.AllResourceConfigs);
			Assert.IsNotEmpty(mConfig.AllResourceConfigs);
			var minId = mConfig.AllResourceConfigs.Select(p => p.Value.Id).Min();
			Assert.Greater(minId, 0);
		}

		[Test]
		public void Get_ReturnsValue()
		{
			var config = mConfig.GetResourceConfig(3);
			Assert.AreEqual(3, config.Id);
			Assert.AreEqual("Coal", config.Name);
			Assert.AreEqual(4f, config.LoadingTime);
		}

		[Test]
		public void Get_Throws_WhenId0Provided()
		{
			var e = Assert.Throws<UnknownResourceException>(() =>
			{
				var config = mConfig.GetResourceConfig(0);
			});

			Assert.AreEqual(0, e.Id);
		}

		[Test]
		public void Get_Throws_WhenIdNonexistentIdProvided()
		{
			var e = Assert.Throws<UnknownResourceException>(() =>
			{
				var config = mConfig.GetResourceConfig(15);
			});

			Assert.AreEqual(15, e.Id);
		}
	}
}