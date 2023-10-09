using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Resources;
using Resources.Configs;

namespace WPFIndustryPresentation.Industry.Implementation
{
	public partial class ResourceWindow : Window
	{
		private static readonly Regex sRegex = new Regex("[^0-9.-]+");

		private readonly IResourcesConfig mResourcesConfig;
		private readonly Dictionary<string, short> mItemTypes;

		public ResourcePackage SelectedResource { get; private set; }

		public ResourceWindow(IResourcesConfig resourcesConfig)
		{
			mResourcesConfig = resourcesConfig;
			InitializeComponent();

			mItemTypes = GetItemTypesAsStringToItemTypeDictionary();

			ItemTypeDropdown.Items.Clear();
			foreach (var type in mItemTypes)
			{
				ItemTypeDropdown.Items.Add(type.Key);
			}
		}

		private Dictionary<string, short> GetItemTypesAsStringToItemTypeDictionary()
		{
			return mResourcesConfig
				.AllResourceConfigs
				.ToDictionary(
					p => p.Value.Name,
					p => p.Key);
		}

		private void OnApplyButtonClicked(object sender, RoutedEventArgs e)
		{
			if (mItemTypes.TryGetValue(ItemTypeDropdown.SelectedItem as string ?? string.Empty, out var itemType)
			    && int.TryParse(AmountTextBox.Text, out var amount))
			{
				SelectedResource = new ResourcePackage(itemType, amount);
				DialogResult = true;
			}
			else
			{
				MessageBox.Show("Could not parse data");
			}
			Close();
		}

		private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !IsTextAllowed(e.Text);
		}

		private static bool IsTextAllowed(string text)
		{
			return !sRegex.IsMatch(text);
		}
	}
}