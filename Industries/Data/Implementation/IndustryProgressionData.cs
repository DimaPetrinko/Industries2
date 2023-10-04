using System;

namespace Industries.Data.Implementation
{
	internal class IndustryProgressionData : IIndustryProgressionData, IIndustryProgressionMutableData
	{
		private byte mLevel;

		public event Action<byte> LevelChanged;

		public byte Level
		{
			get => mLevel;
			set
			{
				var changed = mLevel != value;
				mLevel = value;
				if (changed) LevelChanged?.Invoke(mLevel);
			}
		}

		public IndustryProgressionData()
		{
			Level = 0;
		}
	}
}