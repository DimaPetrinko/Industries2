using System;
using System.Threading.Tasks;
using System.Windows;
using Core.Initialization;
using Core.Initialization.Implementation;
using Industries.Initialization.Implementation;
using WPFEntryPoint.Initialization;
using WPFIndustryPresentation.MainWindow;

namespace WPFEntryPoint
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		private IMainWindowPresenter mMainWindowPresenter;

		public App()
		{
			Initialize();
		}

		private async void Initialize()
		{
			try
			{
				var container = new InitializationContainer();
				await RunInitializers(container);
				container.Flush();
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
				Shutdown(-1);
			}
		}

		private async Task RunInitializers(IInitializationContainer container)
		{
			var initializers = new IInitializer[]
			{
				new ConcurrentInitializer(
					new IndustriesFactoriesInitializer(),
					new IndustriesConfigsInitializer()
				),
				new IndustriesInitializer(),
				new WorldDataInitializer(),
				new MainWindowPresenterInitializer(),
				new SimpleInitializer(GetMainWindowPresenter)
			};

			foreach (var initializer in initializers)
			{
				await initializer.Run(container);
			}
		}

		private void GetMainWindowPresenter(IInitializationContainer container)
		{
			mMainWindowPresenter = container.Get<IMainWindowPresenter>();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			mMainWindowPresenter.Start();
		}
	}
}