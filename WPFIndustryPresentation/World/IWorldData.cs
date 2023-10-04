using System.Collections.Generic;
using Industries.Factories;

namespace WPFIndustryPresentation.World
{
	// TODO: move to Industries. or not? maybe new project. its place is definitely not in presentation
	public interface IWorldData
	{
		IReadOnlyDictionary<short, IndustryHandle> Industries { get; }
		IndustryHandle Get(short id);
	}
}