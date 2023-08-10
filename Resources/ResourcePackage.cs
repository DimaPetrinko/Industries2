using Items;

namespace Resources
{
	public struct ResourcePackage
	{
		public ItemType Type { get; }
		public int Amount { get; }

		public ResourcePackage(ItemType type, int amount)
		{
			Type = type;
			Amount = amount;
		}

		public override string ToString()
		{
			return $"{Type}, {Amount}";
		}
	}
}