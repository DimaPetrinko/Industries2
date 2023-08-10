using System.Collections.Generic;
using System.Linq;

namespace Industries.Configs.Implementation
{
	internal class BinaryIndustryProductionConfig : IIndustryProductionConfig
	{
		private readonly Dictionary<short, Recipe> mRecipes;

		public BinaryIndustryProductionConfig(IEnumerable<Recipe> recipes)
		{
			mRecipes = recipes
				.Select((recipe, i) => new { Index = (short)i, Recipe = recipe })
				.ToDictionary(p => p.Index, p => p.Recipe);
		}

		public Recipe GetRecipeById(short id)
		{
			return mRecipes[id];
		}
	}
}