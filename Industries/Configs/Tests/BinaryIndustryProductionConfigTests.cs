using System.Collections.Generic;
using System.Linq;
using Industries.Configs.Implementation;
using Items;
using NUnit.Framework;
using Resources;

namespace Industries.Configs.Tests
{
	internal class BinaryIndustryProductionConfigTests
	{
		private IIndustryProductionConfig mConfig;

		[SetUp]
		public void SetUp()
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
			mConfig = new BinaryIndustryProductionConfig(recipes);
		}

		[Test]
		public void GetRecipe_ReturnsExistingRecipe()
		{
			Recipe recipe = default;
			Assert.DoesNotThrow(() => recipe = mConfig.GetRecipeById(4));

			Assert.AreEqual(ItemType.Wood, recipe.From.ToArray()[0].Type);
			Assert.AreEqual(1, recipe.From.ToArray()[0].Amount);
			Assert.AreEqual(ItemType.Planks, recipe.To.ToArray()[0].Type);
			Assert.AreEqual(2, recipe.To.ToArray()[0].Amount);
		}

		[Test]
		public void GetRecipe_Throws_WhenNoSuchId()
		{
			Assert.Throws<KeyNotFoundException>(() => mConfig.GetRecipeById(15));
		}
	}
}