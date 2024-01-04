using Core.Time.Implementation;

namespace Core.Time.Factories
{
	public class TimeFactory : ITimeModelFactory
	{
		public ITimeModel CreateStepTimeModel(float timeStep, params ITickable[] tickables)
		{
			var timeModel = new StepTimeModel(timeStep, tickables);
			return timeModel;
		}

		public TimeProvider CreateTimeProvider()
		{
			return new TimeProvider();
		}
	}
}