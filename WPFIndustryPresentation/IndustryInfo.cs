using Industries.Configs;

namespace WPFIndustryPresentation
{
	public readonly struct IndustryInfo
	{
		public readonly short Id;
		public readonly string Name;
		public readonly Recipe Recipe;

		public IndustryInfo(short id, string name, Recipe recipe)
		{
			Id = id;
			Name = name;
			Recipe = recipe;
		}
	}
}