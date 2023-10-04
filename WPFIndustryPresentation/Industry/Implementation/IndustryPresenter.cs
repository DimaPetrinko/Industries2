using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Industries.Data;
using Industries.Exceptions;
using Industries.Model;
using Resources;

namespace WPFIndustryPresentation.Industry.Implementation
{
	public class IndustryPresenter : IIndustryPresenter, IDisposable
	{
		private readonly IndustryInfo mInfo;
		private readonly IIndustry mModel;
		private readonly IIndustryProgression mProgressionModel;
		private readonly IIndustryStateData mStateData;
		private readonly IIndustryStorageData mInputStorageData;
		private readonly IIndustryStorageData mOutputStorageData;
		private readonly IIndustryProgressionData mProgressionData;
		private readonly IIndustryView mView;

		private CancellationTokenSource mProductionCts;
		private CancellationTokenSource mInputLoadingCts;
		private CancellationTokenSource mOutputUnloadingCts;

		public IndustryPresenter(
			IndustryInfo info,
			IIndustry model,
			IIndustryProgression progressionModel,
			IIndustryStateData stateData,
			IIndustryStorageData inputStorageData,
			IIndustryStorageData outputStorageData,
			IIndustryProgressionData progressionData,
			IIndustryView view
		)
		{
			mInfo = info;
			mModel = model;
			mProgressionModel = progressionModel;
			mStateData = stateData;
			mInputStorageData = inputStorageData;
			mOutputStorageData = outputStorageData;
			mProgressionData = progressionData;
			mView = view;

			mStateData.StatusChanged += OnStatusChanged;
			mProgressionData.LevelChanged += OnLevelChanged;

			mInputStorageData.ResourceAdded += OnInputResourcesChanged;
			mInputStorageData.ResourceRemoved += OnInputResourcesChanged;
			mOutputStorageData.ResourceAdded += OnOutputResourcesChanged;
			mOutputStorageData.ResourceRemoved += OnOutputResourcesChanged;

			mView.LevelUpClicked += OnLevelUpClicked;
			mView.ProduceClicked += OnProduceClicked;
			mView.CancelProductionClicked += OnCancelProductionClicked;
			mView.LoadInputClicked += OnLoadInputClicked;
			mView.CancelLoadingInputClicked += OnCancelLoadingInputClicked;
			mView.UnloadOutputClicked += OnUnloadOutputClicked;
			mView.CancelUnloadingOutputClicked += OnCancelUnloadingOutputClicked;
			mView.Window.Closed += OnViewClosed;
		}

		private void OnViewClosed(object sender, EventArgs e)
		{
			Dispose();
		}

		public void Start()
		{
			mView.IndustryName = mInfo.Name;
			OnStatusChanged(mStateData.Status);
			OnLevelChanged(mProgressionData.Level);
			UpdateInputResources();
			UpdateOutputResources();
			mView.Show();
		}

		private void OnStatusChanged(IndustryStatus status)
		{
			try
			{
				mView.Status = status;
				SetUpAllActionButtons(status);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				Console.WriteLine(e);
			}
		}

		private void OnLevelChanged(byte level)
		{
			try
			{
				mView.Level = mProgressionData.Level;
				mView.ToggleLevelUpButton(mProgressionModel.CanLevelUp());
				SetUpAllActionButtons(mStateData.Status);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				Console.WriteLine(e);
			}
		}

		private void SetUpAllActionButtons(IndustryStatus status)
		{
			var isProducing = status == IndustryStatus.Producing
			                  || status == IndustryStatus.UnloadingInput
			                  || status == IndustryStatus.LoadingOutput;
			SetUpActionButton(
				ActionButtonType.Production,
				mModel.CanProduce() || isProducing,
				isProducing);

			var isLoadingInput = status == IndustryStatus.LoadingInput;
			SetUpActionButton(
				ActionButtonType.LoadInput,
				mModel.CanLoadAnyInput() || isLoadingInput,
				isLoadingInput);

			var isUnloadingOutput = status == IndustryStatus.UnloadingOutput;
			SetUpActionButton(
				ActionButtonType.UnloadOutput,
				mModel.CanUnloadAnyOutput() || isUnloadingOutput,
				isUnloadingOutput);
		}

		private void SetUpActionButton(ActionButtonType type, bool isActive, bool isActionOngoing)
		{
			mView.ToggleActionButton(type, isActive);
			var state = isActionOngoing
				? ActionButtonState.StopAction
				: ActionButtonState.StartAction;
			mView.SwitchActionButton(type, state);
		}

		private void OnInputResourcesChanged(ResourcePackage resource)
		{
			UpdateInputResources();
		}

		private void UpdateInputResources()
		{
			try
			{
				mView.InputAmount = mInputStorageData.CurrentAmount;
				mView.InputCapacity = mInputStorageData.CurrentCapacity;
				mView.SetInputResources(mInputStorageData.Resources);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				Console.WriteLine(e);
			}
		}

		private void OnOutputResourcesChanged(ResourcePackage resource)
		{
			UpdateOutputResources();
		}

		private void UpdateOutputResources()
		{
			try
			{
				mView.OutputAmount = mOutputStorageData.CurrentAmount;
				mView.OutputCapacity = mOutputStorageData.CurrentCapacity;
				mView.SetOutputResources(mOutputStorageData.Resources);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				Console.WriteLine(e);
			}
		}

		private void OnLevelUpClicked()
		{
			try
			{
				mProgressionModel.LevelUp();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				Console.WriteLine(e);
			}
		}

		private async void OnProduceClicked()
		{
			try
			{
				mProductionCts = new CancellationTokenSource();
				await Task.Run(() => mModel.Produce(mProductionCts.Token));
				mProductionCts?.Dispose();
				mProductionCts = null;
			}
			catch (InsufficientIndustryLevelException)
			{
				MessageBox.Show("Cannot produce! The level must be at least 0");
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				Console.WriteLine(e);
			}
		}

		private void OnCancelProductionClicked()
		{
			try
			{
				mProductionCts?.Cancel();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				Console.WriteLine(e);
			}
		}

		private async void OnLoadInputClicked(ResourcePackage resource)
		{
			try
			{
				mInputLoadingCts = new CancellationTokenSource();
				await Task.Run(() => mModel.LoadInput(new[] { resource }, mInputLoadingCts.Token));
				mInputLoadingCts?.Dispose();
				mInputLoadingCts = null;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				Console.WriteLine(e);
			}
		}

		private void OnCancelLoadingInputClicked()
		{
			try
			{
				mInputLoadingCts?.Cancel();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				Console.WriteLine(e);
			}
		}

		private async void OnUnloadOutputClicked(ResourcePackage resource)
		{
			try
			{
				// TODO: move opening resources window here and filter shown resources by the recipe
				// TODO: maybe even show the recipe in this window
				mOutputUnloadingCts = new CancellationTokenSource();
				await Task.Run(() => mModel.UnloadOutput(new[] { resource }, mOutputUnloadingCts.Token));
				mOutputUnloadingCts?.Dispose();
				mOutputUnloadingCts = null;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				Console.WriteLine(e);
			}
		}

		private void OnCancelUnloadingOutputClicked()
		{
			try
			{
				mOutputUnloadingCts?.Cancel();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				Console.WriteLine(e);
			}
		}

		public void Dispose()
		{
			mProductionCts?.Cancel();
			mProductionCts?.Dispose();
			mInputLoadingCts?.Cancel();
			mInputLoadingCts?.Dispose();
			mOutputUnloadingCts?.Cancel();
			mOutputUnloadingCts?.Dispose();

			mStateData.StatusChanged -= OnStatusChanged;
			mProgressionData.LevelChanged -= OnLevelChanged;

			mInputStorageData.ResourceAdded -= OnInputResourcesChanged;
			mInputStorageData.ResourceRemoved -= OnInputResourcesChanged;
			mOutputStorageData.ResourceAdded -= OnOutputResourcesChanged;
			mOutputStorageData.ResourceRemoved -= OnOutputResourcesChanged;

			mView.LevelUpClicked -= OnLevelUpClicked;
			mView.ProduceClicked -= OnProduceClicked;
			mView.CancelProductionClicked -= OnCancelProductionClicked;
			mView.LoadInputClicked -= OnLoadInputClicked;
			mView.CancelLoadingInputClicked -= OnCancelLoadingInputClicked;
			mView.UnloadOutputClicked -= OnUnloadOutputClicked;
			mView.CancelUnloadingOutputClicked -= OnCancelUnloadingOutputClicked;
			mView.Window.Closed -= OnViewClosed;
		}
	}
}