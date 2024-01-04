namespace Core.Time
{
	public interface ITimeModel
	{
		float Scale { get; set; }
		void Start();
		void Stop();
	}
}