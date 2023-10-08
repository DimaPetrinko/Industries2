namespace Resources.Configs
{
	public interface IResourceConfig
	{
		short Id { get; }
		string Name { get; }
		float LoadingTime { get; }
	}
}