namespace Resources
{
	public struct ResourcePackage
	{
		public short ResourceId { get; }
		public int Amount { get; }

		public ResourcePackage(short resourceId, int amount)
		{
			ResourceId = resourceId;
			Amount = amount;
		}

		public string ToString(string name)
		{
			return $"{Amount} {name}";
		}
	}
}