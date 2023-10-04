using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Initialization;
using Industries.Configs;
using Industries.Configs.Implementation;
using Items;
using Resources;
using Resources.Configs;
using Resources.Configs.Implementation;

namespace Industries.Initialization.Implementation
{
	public class IndustriesConfigsInitializer : IInitializer
	{
		public Task Run(IInitializationContainer container)
		{
			// TODO: create factories for configs too
			container.Bind(CreateItemsLoadingTimeConfig());
			container.Bind(CreateIndustriesConfig());
			container.Bind(CreateIndustryProductionConfig());
			container.Bind(CreateIndustryProgressionConfig());

			return Task.CompletedTask;
		}

		private static IItemsLoadingTimeConfig CreateItemsLoadingTimeConfig()
		{
			var values = new Dictionary<ItemType, float>
			{
				{ ItemType.None, 0f },
				{ ItemType.Wood, 3f },
				{ ItemType.Planks, 2f },
				{ ItemType.Coal, 4f },
				{ ItemType.Ore, 4f },
				{ ItemType.Metal, 2f },
				{ ItemType.Instruments, 1.5f },
				{ ItemType.Goods, 1.5f },
			};
			return new BinaryItemsLoadingTimeConfig(values);
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
					new[] { new ResourcePackage(ItemType.Wood, 1) }),
				new Recipe(
					new ResourcePackage[] { },
					new[] { new ResourcePackage(ItemType.Coal, 1) }),
				new Recipe(
					new ResourcePackage[] { },
					new[] { new ResourcePackage(ItemType.Ore, 1) }),
				new Recipe(
					new[] { new ResourcePackage(ItemType.Wood, 1) },
					new[] { new ResourcePackage(ItemType.Planks, 2) }),
				new Recipe(
					new[]
					{
						new ResourcePackage(ItemType.Coal, 1),
						new ResourcePackage(ItemType.Ore, 1)
					},
					new[] { new ResourcePackage(ItemType.Metal, 2) }),
				new Recipe(
					new[]
					{
						new ResourcePackage(ItemType.Planks, 1),
						new ResourcePackage(ItemType.Metal, 1)
					},
					new[] { new ResourcePackage(ItemType.Instruments, 2) }),
				new Recipe(
					new[]
					{
						new ResourcePackage(ItemType.Planks, 1),
						new ResourcePackage(ItemType.Metal, 1),
						new ResourcePackage(ItemType.Instruments, 1)
					},
					new[] { new ResourcePackage(ItemType.Goods, 2) }),
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