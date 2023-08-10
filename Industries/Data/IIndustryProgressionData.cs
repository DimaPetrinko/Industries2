using System;

namespace Industries.Data
{
	public interface IIndustryProgressionData
	{
		event Action<int> LevelChanged;

		int Level { get; }
	}
}