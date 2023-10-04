using System;

namespace Industries.Data.Implementation
{
	internal class IndustryStateData : IIndustryStateData, IIndustryStateMutableData
	{
		public event Action<IndustryStatus> StatusChanged;

		private IndustryStatus mStatus;

		public short Id { get; set; }

		public IndustryStatus Status
		{
			get => mStatus;
			set
			{
				var changed = mStatus != value;
				mStatus = value;
				if (changed) StatusChanged?.Invoke(mStatus);
			}
		}

		public IndustryStateData(short id)
		{
			Id = id;
			Status = IndustryStatus.Idle;
		}
	}
}