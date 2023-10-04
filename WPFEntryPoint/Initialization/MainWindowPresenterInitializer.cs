using System.Threading.Tasks;
using Core.Initialization;
using Industries.Configs;
using WPFIndustryPresentation.MainWindow;
using WPFIndustryPresentation.MainWindow.Implementation;
using WPFIndustryPresentation.World;

namespace WPFEntryPoint.Initialization
{
	public class MainWindowPresenterInitializer : IInitializer
	{
		public Task Run(IInitializationContainer container)
		{
			var worldData = container.Get<IWorldData>();
			var industriesConfig = container.Get<IIndustriesConfig>();
			var industryProductionConfig = container.Get<IIndustryProductionConfig>();
			var mainWindowPresenter = new MainWindowPresenter(
				industriesConfig,
				industryProductionConfig,
				worldData,
				new MainWindow());

			container.Bind<IMainWindowPresenter>(mainWindowPresenter);
			return Task.CompletedTask;
		}
	}
}