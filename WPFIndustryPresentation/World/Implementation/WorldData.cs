using System.Collections.Generic;
using Industries.Factories;

namespace WPFIndustryPresentation.World.Implementation
{
	public class WorldData : IWorldData, IWorldMutableData
	{
		IReadOnlyDictionary<short, IndustryHandle> IWorldData.Industries => mIndustries;
		IDictionary<short, IndustryHandle> IWorldMutableData.Industries => mIndustries;

		private readonly Dictionary<short, IndustryHandle> mIndustries = new Dictionary<short, IndustryHandle>();

		public IndustryHandle Get(short id)
		{
			return mIndustries[id];
		}
	}
}