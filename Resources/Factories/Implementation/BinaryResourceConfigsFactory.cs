using System.Collections.Generic;
using Resources.Configs;
using Resources.Configs.Implementation;

namespace Resources.Factories.Implementation
{
	internal class BinaryResourceConfigsFactory : IResourceConfigsFactory
	{
		public IResourceConfig CreateConfig(short id, string name, float loadingTime)
		{
			return new BinaryResourceConfig(id, name, loadingTime);
		}

		public IResourcesConfig CreateResourcesConfig(IEnumerable<IResourceConfig> configs)
		{
			return new BinaryResourcesConfig(configs);
		}
	}
}