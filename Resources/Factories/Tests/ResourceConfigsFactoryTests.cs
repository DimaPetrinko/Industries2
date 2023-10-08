using NUnit.Framework;
using Resources.Factories.Implementation;

namespace Resources.Factories.Tests
{
	internal class ResourceConfigsFactoryTests
	{
		private IResourceConfigsFactory mFactory;

		[SetUp]
		public void SetUp()
		{
			mFactory = new BinaryResourceConfigsFactory();
		}

		[Test]
		public void CreateConfigs_Creates()
		{
			var config = mFactory.CreateConfig(1, "TestResource", 6f);
			Assert.AreEqual(1, config.Id);
			Assert.AreEqual("TestResource", config.Name);
			Assert.AreEqual(6f, config.LoadingTime);
		}

		[Test]
		public void CreateResourcesConfig_Creates()
		{
			var configs = new[]
			{
				mFactory.CreateConfig(1, "1", 1f),
				mFactory.CreateConfig(2, "2", 2f),
				mFactory.CreateConfig(3, "3", 3f),
			};

			var resourcesConfig = mFactory.CreateResourcesConfig(configs);

			Assert.IsNotNull(resourcesConfig.AllResourceConfigs);
			Assert.IsNotEmpty(resourcesConfig.AllResourceConfigs);
			Assert.AreEqual(3, resourcesConfig.AllResourceConfigs.Count);
		}
	}
}