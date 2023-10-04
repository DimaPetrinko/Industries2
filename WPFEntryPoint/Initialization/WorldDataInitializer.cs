using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Initialization;
using Industries.Factories;
using WPFIndustryPresentation.World;
using WPFIndustryPresentation.World.Implementation;

namespace WPFEntryPoint.Initialization
{
	public class WorldDataInitializer : IInitializer
	{
		public Task Run(IInitializationContainer container)
		{
			var worldData = new WorldData();
			var worldMutableData = (IWorldMutableData)worldData;
			var industries = container.Get<IDictionary<short, IndustryHandle>>();
			foreach (var pair in industries)
			{
				worldMutableData.Industries.Add(pair.Key, pair.Value);
			}

			container.Bind<IWorldData>(worldData);
			return Task.CompletedTask;
		}
	}
}