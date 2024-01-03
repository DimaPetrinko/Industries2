using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Time.StepTime.Implementation;
using NUnit.Framework;

namespace Core.Time.StepTime.Tests
{
	internal class StepTimeTests
	{
		private IStepTimeModel mStepTimeModel;

		[SetUp]
		public void SetUp()
		{
			var stepTimeProvider = new StepTimeProvider();
			mStepTimeModel = new StepTimeModel(stepTimeProvider, 1 / 60f);
			Time.RegisterTimeProvider(stepTimeProvider);
		}

		[Test]
		public async Task TimeDelayWhenTickIsNotStartedDoesntProgress()
		{
			var cts = new CancellationTokenSource(200);
			Assert.ThrowsAsync<TaskCanceledException>(async () => await Time.Delay(100, cts.Token));
			Assert.AreEqual(0, Time.SecondsSinceStart);
		}

		[Test]
		public async Task TimeDelayWhenTickIsStartedProgresses()
		{
			mStepTimeModel.StartTicking();
			await Time.Delay(TimeSpan.FromSeconds(0.1f));
			var dT = 1 / 60f;
			Assert.Greater(0.1d + dT, Time.SecondsSinceStart);
			Assert.Less(0.1d - dT, Time.SecondsSinceStart);
			mStepTimeModel.StopTicking();
		}

		[Test]
		public async Task TimeDelayAfterCancelledPreviousDelayStillWorks()
		{
			mStepTimeModel.StartTicking();
			var cts = new CancellationTokenSource(50);
			try
			{
				await Time.Delay(100, cts.Token);
			}
			catch (TaskCanceledException)
			{
				// do nothing
			}
			await Time.Delay(50);
			var dT = 1 / 60f;
			Assert.Greater(0.1d + dT, Time.SecondsSinceStart);
			Assert.Less(0.1d - dT, Time.SecondsSinceStart);
			mStepTimeModel.StopTicking();
		}

		[Test]
		public async Task TimeDelayAfterStoppingTickingDoesntProgress()
		{
			mStepTimeModel.StartTicking();
			await Time.Delay(100);
			mStepTimeModel.StopTicking();
			var cts = new CancellationTokenSource(200);
			Assert.ThrowsAsync<TaskCanceledException>(async () => await Time.Delay(100, cts.Token));
			var dT = 1 / 60f;
			Assert.Greater(0.1d + dT, Time.SecondsSinceStart);
			Assert.Less(0.1d - dT, Time.SecondsSinceStart);
		}

		[Test]
		public void StopTickingWhenNotStartedDoesNotThrow()
		{
			Assert.DoesNotThrow(() => mStepTimeModel.StopTicking());
		}
	}
}