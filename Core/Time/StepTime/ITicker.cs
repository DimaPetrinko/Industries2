namespace Core.Time.StepTime
{
	internal interface ITicker
	{
		void Tick(float deltaTime);
	}
}