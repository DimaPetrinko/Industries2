using Resources;

namespace Industries.Model
{
	public readonly struct ResourcePackageLoadingTime
	{
		public readonly ResourcePackage Resource;
		public readonly float OnePieceLoadingTime;

		public ResourcePackageLoadingTime(ResourcePackage resource, float onePieceLoadingTime)
		{
			Resource = resource;
			OnePieceLoadingTime = onePieceLoadingTime;
		}
	}
}