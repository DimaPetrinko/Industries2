using System;

namespace Industries.Data
{
	public interface IIndustryStateMutableData
	{
		event Action<IndustryStatus> StatusChanged;

		short Id { get; }
		IndustryStatus Status { get; set; }
	}
}