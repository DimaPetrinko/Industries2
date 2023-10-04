using System.Collections.Generic;

namespace Industries.Configs
{
	public interface IIndustriesConfig
	{
		IReadOnlyDictionary<short, IIndustryDataConfig> Industries { get; }
	}
}