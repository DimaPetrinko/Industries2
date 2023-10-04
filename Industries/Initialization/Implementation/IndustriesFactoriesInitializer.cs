using System.Threading.Tasks;
using Core.Initialization;
using Industries.Factories;
using Industries.Factories.Implementation;

namespace Industries.Initialization.Implementation
{
	public class IndustriesFactoriesInitializer : IInitializer
	{
		public Task Run(IInitializationContainer container)
		{
			container.Bind<IIndustryFactory>(new IndustryFactory());
			return Task.CompletedTask;
		}
	}
}