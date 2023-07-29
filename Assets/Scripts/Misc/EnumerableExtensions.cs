using System.Collections.Generic;
using System.Linq;

namespace Misc
{
	public static class EnumerableExtensions
	{
		public static T Random<T>(this IEnumerable<T> enumerable)
		{
			var enumerable1 = enumerable as T[] ?? enumerable.ToArray();
			return enumerable1[UnityEngine.Random.Range(0, enumerable1.Length)];
		}
	}
}