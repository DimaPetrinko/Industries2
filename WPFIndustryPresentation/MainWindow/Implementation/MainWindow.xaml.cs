using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WPFIndustryPresentation.MainWindow.Implementation
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : IMainView
	{
		public event Action<IndustryInfo> Selected;

		private IndustryInfo[] mIndustryInfos;

		public Window Window => this;

		public MainWindow()
		{
			InitializeComponent();
		}

		public void SetIndustries(IEnumerable<IndustryInfo> industryInfos)
		{
			mIndustryInfos = industryInfos.ToArray();
			foreach (var industryInfo in mIndustryInfos)
			{
				IndustriesList.Items.Add(industryInfo.Name);
			}
		}

		private void OnSelectButtonClicked(object sender, RoutedEventArgs e)
		{
			Selected?.Invoke(mIndustryInfos[IndustriesList.SelectedIndex]);
		}
	}
}