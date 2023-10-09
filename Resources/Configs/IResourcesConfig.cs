using System.Collections.Generic;

namespace Resources.Configs
{
	public interface IResourcesConfig
	{
		IReadOnlyDictionary<short, IResourceConfig> AllResourceConfigs { get; }

		IResourceConfig GetResourceConfig(short id);
	}
}