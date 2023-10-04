using System;
using System.Collections.Generic;
using Industries.Data;
using Resources;

namespace WPFIndustryPresentation.Industry
{
	public interface IIndustryView : IWpfView
	{
		event Action LevelUpClicked;
		event Action ProduceClicked;
		event Action CancelProductionClicked;
		event Action<ResourcePackage> LoadInputClicked;
		event Action CancelLoadingInputClicked;
		event Action<ResourcePackage> UnloadOutputClicked;
		event Action CancelUnloadingOutputClicked;

		string IndustryName { set; }
		IndustryStatus Status { set; }
		byte Level { set; }
		int InputAmount { set; }
		int InputCapacity { set; }
		int OutputAmount { set; }
		int OutputCapacity { set; }

		void SetInputResources(IEnumerable<ResourcePackage> resources);
		void SetOutputResources(IEnumerable<ResourcePackage> resources);
		void ToggleLevelUpButton(bool value);
		void ToggleActionButton(ActionButtonType type, bool active);
		void SwitchActionButton(ActionButtonType type, ActionButtonState state);
	}
}