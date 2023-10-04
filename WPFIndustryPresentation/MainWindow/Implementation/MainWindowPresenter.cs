using System.Linq;
using Industries.Configs;
using WPFIndustryPresentation.Industry.Implementation;
using WPFIndustryPresentation.World;

namespace WPFIndustryPresentation.MainWindow.Implementation
{
	public class MainWindowPresenter : IMainWindowPresenter
	{
		private readonly IIndustriesConfig mIndustriesConfig;
		private readonly IIndustryProductionConfig mProductionConfig;
		private readonly IWorldData mWorldData;
		private readonly IMainView mView;

		public MainWindowPresenter(
			IIndustriesConfig industriesConfig,
			IIndustryProductionConfig productionConfig,
			IWorldData worldData,
			IMainView view
		)
		{
			mIndustriesConfig = industriesConfig;
			mProductionConfig = productionConfig;
			mWorldData = worldData;
			mView = view;
			mView.Selected += OnSelected;
		}

		public void Start()
		{
			var industryInfos = mWorldData
				.Industries
				.Select(p =>
				{
					var industryDataConfig = mIndustriesConfig.Industries[p.Key];
					return new IndustryInfo(
						p.Key,
						industryDataConfig.Name,
						mProductionConfig.GetRecipeById(industryDataConfig.RecipeId));
				});
			mView.SetIndustries(industryInfos);
			mView.Show();
		}

		private void OnSelected(IndustryInfo industryInfo)
		{
			var industryWindow = new IndustryWindow();
			industryWindow.Owner = mView.Window;
			var industryHandle = mWorldData.Get(industryInfo.Id);
			var industryPresenter = new IndustryPresenter(
				industryInfo,
				industryHandle.Industry,
				industryHandle.Progression,
				industryHandle.StateData,
				industryHandle.InputStorageData,
				industryHandle.OutputStorageData,
				industryHandle.ProgressionData,
				industryWindow
			);

			industryPresenter.Start();
		}
	}
}