using Items;

namespace Resources.Configs
{
	public interface IItemsLoadingTimeConfig
	{
		float GetLoadingTimeForItemType(ItemType itemType);
	}
}