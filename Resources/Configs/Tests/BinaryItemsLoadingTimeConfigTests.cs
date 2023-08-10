using System.Collections.Generic;
using Items;
using NUnit.Framework;
using Resources.Configs.Implementation;

namespace Resources.Configs.Tests
{
	public class BinaryItemsLoadingTimeConfigTests
	{
		[Test]
		public void Get_ReturnsValue()
		{
			var values = new Dictionary<ItemType, float>
			{
				{ ItemType.None, 0f },
				{ ItemType.Wood, 3f },
				{ ItemType.Planks, 2f },
				{ ItemType.Coal, 4f },
				{ ItemType.Ore, 4f },
				{ ItemType.Metal, 2f },
				{ ItemType.Instruments, 1.5f },
				{ ItemType.Goods, 1.5f },
			};
			var config = new BinaryItemsLoadingTimeConfig(values);

			Assert.AreEqual(3f, config.GetLoadingTimeForItemType(ItemType.Wood));
		}
		[Test]
		public void Get_Returns0_IfTimeNotProvided()
		{
			var values = new Dictionary<ItemType, float>
			{
				{ ItemType.Planks, 2f },
				{ ItemType.Metal, 2f },
				{ ItemType.Instruments, 1.5f },
				{ ItemType.Goods, 1.5f },
			};
			var config = new BinaryItemsLoadingTimeConfig(values);

			Assert.AreEqual(0f, config.GetLoadingTimeForItemType(ItemType.Wood));
			Assert.AreEqual(0f, config.GetLoadingTimeForItemType(ItemType.Coal));
			Assert.AreEqual(0f, config.GetLoadingTimeForItemType(ItemType.Ore));
		}
	}
}