using System.Collections.Generic;
using Resources.Configs;

namespace Resources.Factories
{
	public interface IResourceConfigsFactory
	{
		IResourceConfig CreateConfig(short id, string name, float loadingTime);
		IResourcesConfig CreateResourcesConfig(IEnumerable<IResourceConfig> configs);
	}
}