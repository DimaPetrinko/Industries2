using System;
using System.Collections.Generic;
using System.Linq;
using Items;

namespace Resources.Configs.Implementation
{
	public class BinaryItemsLoadingTimeConfig : IItemsLoadingTimeConfig
	{
		private readonly Dictionary<ItemType, float> mValues;

		public BinaryItemsLoadingTimeConfig(IReadOnlyDictionary<ItemType, float> values)
		{
			mValues = Enum.GetValues(typeof(ItemType)).Cast<ItemType>()
				.ToDictionary(t => t, t => values.TryGetValue(t, out var value) ? value : 0f);
		}

		public float GetLoadingTimeForItemType(ItemType itemType)
		{
			return mValues[itemType];
		}
	}
}