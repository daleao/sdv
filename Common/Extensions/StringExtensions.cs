using System;
using System.Linq;

namespace TheLion.Common
{
	public static class StringExtensions
	{
		/// <summary>Capitalize the first character in the calling string.</summary>
		public static string FirstCharToUpper(this string s)
		{
			return s switch
			{
				null => throw new ArgumentNullException(nameof(s)),
				"" => throw new ArgumentException($"{nameof(s)} cannot be empty."),
				_ => s.First().ToString().ToUpper() + s.Substring(1)
			};
		}
	}
}