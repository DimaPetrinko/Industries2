using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Initialization;
using Industries.Configs;
using Industries.Configs.Implementation;
using Resources;

namespace Industries.Initialization.Implementation
{
	public class IndustriesConfigsInitializer : IInitializer
	{
		public Task Run(IInitializationContainer container)
		{
			// TODO: create factories for configs too
			container.Bind(CreateIndustriesConfig());
			container.Bind(CreateIndustryProductionConfig());
			container.Bind(CreateIndustryProgressionConfig());

			return Task.CompletedTask;
		}

		private static IIndustriesConfig CreateIndustriesConfig()
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

			return new BinaryIndustriesConfig(configs);
		}

		private static IIndustryProductionConfig CreateIndustryProductionConfig()
		{
			var recipes = new[]
			{
				new Recipe(
					new ResourcePackage[] { },
					new[] { new ResourcePackage(1, 1) }),
				new Recipe(
					new ResourcePackage[] { },
					new[] { new ResourcePackage(3, 1) }),
				new Recipe(
					new ResourcePackage[] { },
					new[] { new ResourcePackage(4, 1) }),
				new Recipe(
					new[] { new ResourcePackage(1, 1) },
					new[] { new ResourcePackage(2, 2) }),
				new Recipe(
					new[]
					{
						new ResourcePackage(3, 1),
						new ResourcePackage(4, 1)
					},
					new[] { new ResourcePackage(5, 2) }),
				new Recipe(
					new[]
					{
						new ResourcePackage(2, 1),
						new ResourcePackage(5, 1)
					},
					new[] { new ResourcePackage(6, 2) }),
				new Recipe(
					new[]
					{
						new ResourcePackage(2, 1),
						new ResourcePackage(5, 1),
						new ResourcePackage(6, 1)
					},
					new[] { new ResourcePackage(7, 2) }),
			};
			return new BinaryIndustryProductionConfig(recipes);
		}

		private static IIndustryProgressionConfig CreateIndustryProgressionConfig()
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
			return new BinaryIndustryProgressionConfig(levelConfigs);
		}
	}
}