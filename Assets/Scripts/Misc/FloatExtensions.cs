using UnityEngine;

namespace Misc
{
	public static class FloatExtensions
	{
		public static  bool IsApproximately(this float a, float b, float tolerance = 0.01f)
		{
			return Mathf.Abs(a - b) < tolerance;
		}
		
	}
}