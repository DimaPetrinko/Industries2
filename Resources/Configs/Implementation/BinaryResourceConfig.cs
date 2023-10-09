namespace Resources.Configs.Implementation
{
	internal class BinaryResourceConfig : IResourceConfig
	{
		public short Id { get; }
		public string Name { get; }
		public float LoadingTime { get; }

		public BinaryResourceConfig(short id, string name, float loadingTime)
		{
			Id = id;
			Name = name;
			LoadingTime = loadingTime;
		}
	}
}