namespace Industries.Configs.Implementation
{
	internal class BinaryIndustryDataConfig : IIndustryDataConfig
	{
		public string Name { get; }
		public short RecipeId { get; }

		public BinaryIndustryDataConfig(
			string name,
			short recipeId
		)
		{
			Name = name;
			RecipeId = recipeId;
		}
	}
}