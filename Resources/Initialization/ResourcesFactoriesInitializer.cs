using System.Threading.Tasks;
using Core.Initialization;
using Resources.Factories;
using Resources.Factories.Implementation;

namespace Resources.Initialization
{
	public class ResourcesFactoriesInitializer : IInitializer
	{
		public Task Run(IInitializationContainer container)
		{
			container.Bind<IResourceConfigsFactory>(new BinaryResourceConfigsFactory());

			return Task.CompletedTask;
		}
	}
}