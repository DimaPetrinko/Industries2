namespace Core.Time.Factories
{
	public interface ITimeModelFactory
	{
		ITimeModel CreateStepTimeModel(float timeStep, params ITickable[] tickables);
		TimeProvider CreateTimeProvider();
	}
}