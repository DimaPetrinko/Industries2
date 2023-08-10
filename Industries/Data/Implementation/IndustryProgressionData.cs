using System;

namespace Industries.Data.Implementation
{
	public class IndustryProgressionData : IIndustryProgressionData, IIndustryProgressionMutableData
	{
		private int mLevel;
		public event Action<int> LevelChanged;

		public int Level
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