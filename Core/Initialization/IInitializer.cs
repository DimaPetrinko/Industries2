using System.Threading.Tasks;

namespace Core.Initialization
{
	public interface IInitializer
	{
		Task Run(IInitializationContainer container);
	}
}