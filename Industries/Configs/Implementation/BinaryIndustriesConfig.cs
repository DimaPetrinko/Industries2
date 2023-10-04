using System.Collections.Generic;

namespace Industries.Configs.Implementation
{
	internal class BinaryIndustriesConfig : IIndustriesConfig
	{
		public IReadOnlyDictionary<short, IIndustryDataConfig> Industries { get; }

		public BinaryIndustriesConfig(IReadOnlyDictionary<short, IIndustryDataConfig> industries)
		{
			Industries = industries;
		}
	}
}