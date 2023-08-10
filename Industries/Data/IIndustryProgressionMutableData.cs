using System;

namespace Industries.Data
{
	public interface IIndustryProgressionMutableData
	{
		event Action<int> LevelChanged;
		int Level { get; set; }
	}
}