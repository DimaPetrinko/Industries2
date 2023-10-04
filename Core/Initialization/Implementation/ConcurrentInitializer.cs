using System.Linq;
using System.Threading.Tasks;

namespace Core.Initialization.Implementation
{
	public class ConcurrentInitializer : IInitializer
	{
		private readonly IInitializer[] mInitializers;

		public ConcurrentInitializer(params IInitializer[] initializers)
		{
			mInitializers = initializers;
		}

		public async Task Run(IInitializationContainer container)
		{
			await Task.WhenAll(mInitializers.Select(i => i.Run(container)));
		}
	}
}