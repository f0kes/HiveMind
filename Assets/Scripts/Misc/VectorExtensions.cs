using UnityEngine;

namespace Misc
{
	public static class VectorExtensions
	{
		public static Vector2 NormalizeClamp(this ref Vector2 vector, float maxVal = 1)
		{
			vector = vector.magnitude >= maxVal ? vector / vector.magnitude * maxVal : vector;
			return vector;
		}

		public static Vector3 NormalizeClamp(this ref Vector3 vector, float maxVal = 1)
		{
			vector = vector.magnitude >= maxVal ? vector / vector.magnitude * maxVal : vector;
			return vector;
		}
	}
}