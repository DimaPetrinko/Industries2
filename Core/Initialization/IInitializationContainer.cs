namespace Core.Initialization
{
	public interface IInitializationContainer
	{
		void Bind<T>(T instance);
		T Get<T>();
		bool Has<T>();
		void Flush();
	}
}