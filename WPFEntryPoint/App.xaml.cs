using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Core.Initialization;
using Core.Initialization.Implementation;
using Industries.Initialization.Implementation;
using Resources.Initialization;
using WPFEntryPoint.Initialization;
using WPFIndustryPresentation.MainWindow;

namespace WPFEntryPoint
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		private Task mInitializationTask;
		private IMainWindowPresenter mMainWindowPresenter;

		public App()
		{
			Initialize();
		}

		private async void Initialize()
		{
			mInitializationTask = ExecuteInitialization();
			await mInitializationTask;
		}

		private async Task ExecuteInitialization()
		{
			try
			{
				var systemContainer = new InitializationContainer();
				await Task.Run(() => RunSystemInitializers(systemContainer));
				await RunPresentationInitializers(systemContainer);
				systemContainer.Flush();
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.ToString());
				MessageBox.Show(exception.Message);
			}
		}

		private async Task RunSystemInitializers(IInitializationContainer container)
		{
			var initializers = new IInitializer[]
			{
				new ConcurrentInitializer(
					new ResourcesFactoriesInitializer(),
					new IndustriesFactoriesInitializer()
				),
				new ConcurrentInitializer(
					new ResourcesConfigsInitializer(),
					new IndustriesConfigsInitializer()
				),
				new IndustriesInitializer(),
				new WorldDataInitializer()
			};

			foreach (var initializer in initializers)
			{
				await initializer.Run(container);
			}
		}

		private async Task RunPresentationInitializers(IInitializationContainer container)
		{
			Console.WriteLine(Thread.CurrentThread.ManagedThreadId);

			var initializers = new IInitializer[]
			{
				new MainWindowPresenterInitializer(),
				new SimpleInitializer(GetMainWindowPresenter)
			};

			await Dispatcher.Invoke(async () =>
			{
				foreach (var initializer in initializers)
				{
					await initializer.Run(container);
				}
			});
		}

		private void GetMainWindowPresenter(IInitializationContainer container)
		{
			mMainWindowPresenter = container.Get<IMainWindowPresenter>();
		}

		protected override async void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			await mInitializationTask;

			if (mMainWindowPresenter != null)
				mMainWindowPresenter.Start();
			else
				Shutdown(-1);
		}
	}
}