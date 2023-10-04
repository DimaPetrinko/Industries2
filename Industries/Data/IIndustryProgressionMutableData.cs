using System;

namespace Industries.Data
{
	public interface IIndustryProgressionMutableData
	{
		event Action<byte> LevelChanged;

		byte Level { get; set; }
	}
}