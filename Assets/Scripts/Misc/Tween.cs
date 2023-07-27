using UnityEngine;

namespace Misc
{
	public static class Tween
	{
		public static float BezierBlend(float t, float p0 = 3f, float p1 = 2f)
		{
			return Mathf.Clamp01(t * t * (p0 - p1 * t));
		}
		public static float EaseInSine(float t)
		{
			t = Mathf.Clamp01(t);
			return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
		}
		public static float EaseOutSine(float t)
		{
			t = Mathf.Clamp01(t);
			return Mathf.Sin(t * Mathf.PI * 0.5f);
		}
	}
}