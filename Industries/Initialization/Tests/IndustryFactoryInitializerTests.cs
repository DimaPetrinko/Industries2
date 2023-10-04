using System.Threading.Tasks;
using Core.Initialization.Implementation;
using Industries.Factories;
using Industries.Initialization.Implementation;
using NUnit.Framework;

namespace Industries.Initialization.Tests
{
	internal class IndustryFactoryInitializerTests
	{
		[Test]
		public void Constructor_Creates()
		{
			Assert.DoesNotThrow(() =>
			{
				var initializer = new IndustriesFactoriesInitializer();
			});
		}

		[Test]
		public async Task Run_SavesNewFactoryToData()
		{
			var container = new InitializationContainer();

			Assert.IsFalse(container.Has<IIndustryFactory>());

			var initializer = new IndustriesFactoriesInitializer();
			await initializer.Run(container);

			Assert.IsTrue(container.Has<IIndustryFactory>());
		}
	}
}