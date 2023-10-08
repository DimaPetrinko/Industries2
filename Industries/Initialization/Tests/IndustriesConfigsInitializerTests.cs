using System.Threading.Tasks;
using Core.Initialization.Implementation;
using Industries.Configs;
using Industries.Initialization.Implementation;
using NUnit.Framework;

namespace Industries.Initialization.Tests
{
	internal class IndustriesConfigsInitializerTests
	{
		[Test]
		public void Constructor_Creates()
		{
			Assert.DoesNotThrow(() =>
			{
				var initializer = new IndustriesConfigsInitializer();
			});
		}

		[Test]
		public async Task Run_SavesConfigsToData()
		{
			var container = new InitializationContainer();

			Assert.IsFalse(container.Has<IIndustriesConfig>());
			Assert.IsFalse(container.Has<IIndustryProductionConfig>());
			Assert.IsFalse(container.Has<IIndustryProgressionConfig>());

			var initializer = new IndustriesConfigsInitializer();
			await initializer.Run(container);

			Assert.IsTrue(container.Has<IIndustriesConfig>());
			Assert.IsTrue(container.Has<IIndustryProductionConfig>());
			Assert.IsTrue(container.Has<IIndustryProgressionConfig>());
		}
	}
}