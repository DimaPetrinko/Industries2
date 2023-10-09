using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Industries.Data;
using Resources;
using Resources.Configs;
using WPFIndustryPresentation.Utilities;

namespace WPFIndustryPresentation.Industry.Implementation
{
	public partial class IndustryWindow : IIndustryView
	{
		public event Action LevelUpClicked;
		public event Action ProduceClicked;
		public event Action CancelProductionClicked;
		public event Action<ResourcePackage> LoadInputClicked;
		public event Action CancelLoadingInputClicked;
		public event Action<ResourcePackage> UnloadOutputClicked;
		public event Action CancelUnloadingOutputClicked;

		private const string cCurrentAmountText = "Current amount: {0}";
		private const string cCapacityText = "Capacity: {0}";

		private readonly IResourcesConfig mResourcesConfig;
		private readonly Dictionary<ActionButtonType, ActionButton> mActionButtons;

		public Window Window => this;

		public string IndustryName
		{
			set => Dispatcher.Invoke(() => IndustryNameLabel.Content = value);
		}

		public IndustryStatus Status
		{
			set => Dispatcher.Invoke(() =>
				StatusLabel.Content = $"Status: {StringUtilities.SplitCamelCase(value.ToString())}");
		}

		public byte Level
		{
			set => Dispatcher.Invoke(() => LevelLabel.Content = $"Level: {value}");
		}

		public int InputAmount
		{
			set => Dispatcher.Invoke(() => InputAmountLabel.Content = string.Format(cCurrentAmountText, value));
		}

		public int InputCapacity
		{
			set => Dispatcher.Invoke(() => InputCapacityLabel.Content = string.Format(cCapacityText, value));
		}

		public int OutputAmount
		{
			set => Dispatcher.Invoke(() => OutputAmountLabel.Content = string.Format(cCurrentAmountText, value));
		}

		public int OutputCapacity
		{
			set => Dispatcher.Invoke(() => OutputCapacityLabel.Content = string.Format(cCapacityText, value));
		}

		public IndustryWindow(IResourcesConfig resourcesConfig)
		{
			InitializeComponent();

			mResourcesConfig = resourcesConfig;
			mActionButtons = new Dictionary<ActionButtonType, ActionButton>
			{
				{ ActionButtonType.Production, new ActionButton(ProduceButton, CancelProductionButton) },
				{ ActionButtonType.LoadInput, new ActionButton(LoadInputButton, CancelLoadingInputButton) },
				{ ActionButtonType.UnloadOutput, new ActionButton(UnloadOutputButton, CancelUnloadingOutputButton) }
			};
		}

		public void SetInputResources(IEnumerable<ResourcePackage> resources)
		{
			Dispatcher.Invoke(() =>
			{
				InputResources.Items.Clear();
				foreach (var resource in resources.Where(r => r.Amount > 0))
				{
					InputResources.Items.Add(resource.ToString());
				}
			});
		}

		public void SetOutputResources(IEnumerable<ResourcePackage> resources)
		{
			Dispatcher.Invoke(() =>
			{
				OutputResources.Items.Clear();
				foreach (var resource in resources.Where(r => r.Amount > 0))
				{
					OutputResources.Items.Add(resource.ToString());
				}
			});
		}

		public void ToggleLevelUpButton(bool value)
		{
			Dispatcher.Invoke(() => LevelUpButton.IsEnabled = value);
		}

		public void ToggleActionButton(ActionButtonType type, bool active)
		{
			Dispatcher.Invoke(() =>
			{
				if (mActionButtons.TryGetValue(type, out var actionButton))
					actionButton.Toggle(active);
				else
					Console.WriteLine($"Warning: cannot find action button of type {type}");
			});
		}

		public void SwitchActionButton(ActionButtonType type, ActionButtonState state)
		{
			Dispatcher.Invoke(() =>
			{
				if (mActionButtons.TryGetValue(type, out var actionButton))
					actionButton.Switch(state);
				else
					Console.WriteLine($"Warning: cannot find action button of type {type}");
			});
		}

		private void OnLevelUpButtonClicked(object sender, RoutedEventArgs e)
		{
			LevelUpClicked?.Invoke();
		}

		private void OnProduceButtonClicked(object sender, RoutedEventArgs e)
		{
			ProduceClicked?.Invoke();
		}

		private void OnCancelProductionButtonClicked(object sender, RoutedEventArgs e)
		{
			CancelProductionClicked?.Invoke();
		}

		private void OnLoadInputButtonClicked(object sender, RoutedEventArgs e)
		{
			var resourceWindow = new ResourceWindow(mResourcesConfig);
			if (resourceWindow.ShowDialog().GetValueOrDefault())
			{
				LoadInputClicked?.Invoke(resourceWindow.SelectedResource);
			}
		}

		private void OnCancelLoadingInputButtonClicked(object sender, RoutedEventArgs e)
		{
			CancelLoadingInputClicked?.Invoke();
		}

		private void OnUnloadOutputButtonClicked(object sender, RoutedEventArgs e)
		{
			var resourceWindow = new ResourceWindow(mResourcesConfig);
			if (resourceWindow.ShowDialog().GetValueOrDefault())
			{
				UnloadOutputClicked?.Invoke(resourceWindow.SelectedResource);
			}
		}

		private void OnCancelUnloadingOutputButtonClicked(object sender, RoutedEventArgs e)
		{
			CancelUnloadingOutputClicked?.Invoke();
		}
	}
}