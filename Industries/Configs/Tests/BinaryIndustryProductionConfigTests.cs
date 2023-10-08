using System.Collections.Generic;
using System.Linq;
using Industries.Configs.Implementation;
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
			mConfig = new BinaryIndustryProductionConfig(recipes);
		}

		[Test]
		public void GetRecipe_ReturnsExistingRecipe()
		{
			Recipe recipe = default;
			Assert.DoesNotThrow(() => recipe = mConfig.GetRecipeById(4));

			Assert.AreEqual(1, recipe.From.ToArray()[0].ResourceId);
			Assert.AreEqual(1, recipe.From.ToArray()[0].Amount);
			Assert.AreEqual(2, recipe.To.ToArray()[0].ResourceId);
			Assert.AreEqual(2, recipe.To.ToArray()[0].Amount);
		}

		[Test]
		public void GetRecipe_Throws_WhenNoSuchId()
		{
			Assert.Throws<KeyNotFoundException>(() => mConfig.GetRecipeById(15));
		}
	}
}