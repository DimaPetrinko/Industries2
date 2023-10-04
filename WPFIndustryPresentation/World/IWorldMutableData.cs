using System.Collections.Generic;
using Industries.Factories;

namespace WPFIndustryPresentation.World
{
	public interface IWorldMutableData
	{
		IDictionary<short, IndustryHandle> Industries { get; }
	}
}