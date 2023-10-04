using System;

namespace Industries.Data
{
	public interface IIndustryProgressionData
	{
		event Action<byte> LevelChanged;

		byte Level { get; }
	}
}