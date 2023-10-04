using System;
using System.Collections.Generic;

namespace WPFIndustryPresentation.MainWindow
{
	public interface IMainView : IWpfView
	{
		event Action<IndustryInfo> Selected;

		void SetIndustries(IEnumerable<IndustryInfo> industryInfos);
	}
}