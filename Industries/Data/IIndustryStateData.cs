using System;

namespace Industries.Data
{
	public interface IIndustryStateData
	{
		event Action<IndustryStatus> StatusChanged;

		// TODO: unused
		short Id { get; }
		IndustryStatus Status { get; }
	}
}