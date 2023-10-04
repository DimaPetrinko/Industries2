using System;
using System.Threading.Tasks;

namespace Core.Initialization.Implementation
{
	public class SimpleInitializer : IInitializer
	{
		private readonly Action<IInitializationContainer> mAction;

		public SimpleInitializer(Action<IInitializationContainer> action)
		{
			mAction = action;
		}

		public Task Run(IInitializationContainer container)
		{
			mAction?.Invoke(container);
			return Task.CompletedTask;
		}
	}
}