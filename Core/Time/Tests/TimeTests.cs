using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Time.Implementation;
using NUnit.Framework;

namespace Core.Time.Tests
{
	internal class TimeTests
	{
		private ITimeModel mStepTimeModel;

		[SetUp]
		public void SetUp()
		{
			var timeProvider = new TimeProvider();
			mStepTimeModel = new StepTimeModel(1 / 60f, timeProvider);
			Time.Initialize(mStepTimeModel, timeProvider);
			Time.Scale = 1;
		}

		[Test]
		public async Task TimeDelay_WhenTickIsNotStarted_DoesntProgress()
		{
			var cts = new CancellationTokenSource(200);
			Assert.ThrowsAsync<TaskCanceledException>(async () => await Time.Delay(100, cts.Token));
			Assert.AreEqual(0, Time.SecondsSinceStart);
		}

		[Test]
		public async Task TimeDelay_WhenTickIsStarted_Progresses()
		{
			Time.Start();
			await Time.Delay(TimeSpan.FromSeconds(0.1f), p => {});
			var dT = 1 / 60f;
			Assert.Greater(0.1d + dT, Time.SecondsSinceStart);
			Assert.Less(0.1d - dT, Time.SecondsSinceStart);
			Time.Stop();
		}

		[Test]
		public async Task TimeDelay_AfterCancelledPreviousDelay_StillWorks()
		{
			Time.Start();
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
			Time.Stop();
		}

		[Test]
		public async Task TimeDelay_AfterStoppingTicking_DoesntProgress()
		{
			Time.Start();
			await Time.Delay(100);
			Time.Stop();
			var cts = new CancellationTokenSource(200);
			Assert.ThrowsAsync<TaskCanceledException>(async () => await Time.Delay(100, cts.Token));
			var dT = 1 / 60f;
			Assert.Greater(0.1d + dT, Time.SecondsSinceStart);
			Assert.Less(0.1d - dT, Time.SecondsSinceStart);
		}

		[Test]
		public void StopTicking_WhenNotStarted_DoesNotThrow()
		{
			Assert.DoesNotThrow(() => mStepTimeModel.Stop());
		}

		[Test]
		public async Task TimeDelay_WhenScaleIs10_Takes10TimesLessTime()
		{
			Time.Scale = 10;
			Time.Start();
			await Task.WhenAll(
				Time.Delay(100),
				TaskDelay()
			);

			async Task TaskDelay()
			{
				await Task.Delay(100);
				Console.WriteLine(Time.SecondsSinceStart);
				var dt = 1f / 120 / 10;
				Assert.Greater(0.01f + 3 * dt, Time.SecondsSinceStart);
				Assert.Less(0.01f - 3 * dt, Time.SecondsSinceStart);
				Console.WriteLine(Time.Scale);
			}
		}

		[Test]
		public async Task OnTick_WhenTriggered_WhenCorrectData()
		{
			Time.Start();
			var lastProgress = -1f;

			await Time.Delay(100, OnTick);

			void OnTick(float progress)
			{
				Assert.Greater(progress, lastProgress);
				Assert.GreaterOrEqual(progress, 0);
				Assert.LessOrEqual(progress, 1);
				lastProgress = progress;
			}
		}
	}
}