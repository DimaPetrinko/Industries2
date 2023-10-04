using System.Windows;

namespace WPFIndustryPresentation
{
	public interface IWpfView
	{
		Window Window { get; }
		void Show();
		bool? ShowDialog();
	}
}