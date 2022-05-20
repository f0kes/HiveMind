using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
	public static class CircleDivider
	{
		public static List<Vector2> GetDivisions(int divisions)
		{
			List<Vector2> result = new List<Vector2>();
			for (int i = 0; i < divisions; i++)
			{
				var angle = i * Mathf.PI * 2f / divisions;
				var newPos = new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
				result.Add(newPos);
			}

			return result;
		}

		public static Vector3 XZ(this Vector2 vector2)
		{
			return new Vector3(vector2.x, 0, vector2.y);
		}
		public static Vector3 XZ(this Vector3 vector2)
		{
			return new Vector3(vector2.x, 0, vector2.y);
		}

	}
}