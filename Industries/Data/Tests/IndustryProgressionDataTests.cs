using Industries.Data.Implementation;
using NUnit.Framework;

namespace Industries.Data.Tests
{
	internal class IndustryProgressionDataTests
	{
		[Test]
		public void Constructor_SetsDefaultValues()
		{
			var data = new IndustryProgressionData();
			Assert.AreEqual(0, data.Level);
		}

		[Test]
		public void SetLevel_SetsValue()
		{
			var data = new IndustryProgressionData();
			data.Level = 5;
			Assert.AreEqual(5, data.Level);
		}
	}
}