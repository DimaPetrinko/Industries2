using System.Collections.Generic;
using Resources;

namespace Industries.Configs
{
	public readonly struct Recipe
	{
		public readonly IEnumerable<ResourcePackage> From;
		public readonly IEnumerable<ResourcePackage> To;

		public Recipe(IEnumerable<ResourcePackage> from, IEnumerable<ResourcePackage> to)
		{
			From = from;
			To = to;
		}
	}
}