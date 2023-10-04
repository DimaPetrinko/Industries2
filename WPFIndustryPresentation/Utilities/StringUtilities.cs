using System.Text.RegularExpressions;

namespace WPFIndustryPresentation.Utilities
{
	internal static class StringUtilities
	{
		public static string SplitCamelCase(string input)
		{
			return Regex.Replace(input, @"\B\p{Lu}", m => $" {m.ToString().ToLower()}");
		}
	}
}