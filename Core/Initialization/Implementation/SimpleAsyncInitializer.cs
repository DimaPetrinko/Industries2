using System;
using System.Threading.Tasks;

namespace Core.Initialization.Implementation
{
	public class SimpleAsyncInitializer : IInitializer
	{
		private readonly Func<IInitializationContainer, Task> mTask;

		public SimpleAsyncInitializer(Func<IInitializationContainer, Task> task)
		{
			mTask = task;
		}

		public async Task Run(IInitializationContainer container)
		{
			await mTask(container);
		}
	}
}