using System;
using System.Collections.Generic;
using System.Linq;
using Resources.Exceptions;

namespace Resources.Configs.Implementation
{
	internal class BinaryResourcesConfig : IResourcesConfig
	{
		private readonly Dictionary<short, IResourceConfig> mItems;

		public IReadOnlyDictionary<short, IResourceConfig> AllResourceConfigs => mItems;

		public BinaryResourcesConfig(IEnumerable<IResourceConfig> itemConfigs)
		{
			var itemConfigsArray = itemConfigs as IResourceConfig[] ?? itemConfigs.ToArray();
			if (itemConfigsArray.Any(c => c == null))
				throw new ArgumentException("One of configs are null");
			try
			{
				mItems = itemConfigsArray.ToDictionary(c => c.Id, c => c);
			}
			catch (ArgumentException)
			{
				var duplicateId = itemConfigsArray
					.Select(c => c.Id)
					.GroupBy(g => g)
					.FirstOrDefault(g => g.Count() > 1)?.Key ?? 0;
				throw new DuplicateResourceIdsException(duplicateId);
			}
		}

		public IResourceConfig GetResourceConfig(short id)
		{
			if (mItems.TryGetValue(id, out var config))
			{
				return config;
			}

			throw new UnknownResourceException(id);
		}
	}
}