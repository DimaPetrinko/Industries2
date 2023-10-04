using System.Windows;
using System.Windows.Controls;

namespace WPFIndustryPresentation.Industry.Implementation
{
	internal class ActionButton
	{
		private readonly Button mStartButton;
		private readonly Button mStopButton;

		public ActionButton(
			Button startButton,
			Button stopButton
		)
		{
			mStartButton = startButton;
			mStopButton = stopButton;
		}
		public void Toggle(bool value)
		{
			mStartButton.IsEnabled = value;
			mStopButton.IsEnabled = value;
		}

		public void Switch(ActionButtonState state)
		{
			if (state == ActionButtonState.StartAction)
			{
				mStartButton.Visibility = Visibility.Visible;
				mStopButton.Visibility = Visibility.Hidden;
			}
			else
			{
				mStartButton.Visibility = Visibility.Hidden;
				mStopButton.Visibility = Visibility.Visible;
			}
		}
	}
}