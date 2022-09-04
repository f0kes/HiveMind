using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tools
{
	public class CircleDivider
	{
		private static Dictionary<int, CircleDivider> _instances = new Dictionary<int, CircleDivider>();

		private Vector2[] _directions;

		private Dictionary<Vector2, int> _circlePointsIndex = new Dictionary<Vector2, int>();

		public CircleDivider(int divisions)
		{
			if (_instances.ContainsKey(divisions))
			{
				_directions = _instances[divisions]._directions;
				_circlePointsIndex = _instances[divisions]._circlePointsIndex;
				return;
			}

			_directions = new Vector2[divisions];
			for (int i = 0; i < divisions; i++)
			{
				var angle = i * Mathf.PI * 2f / divisions;
				var newPos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

				_circlePointsIndex.Add(newPos, i);
				_directions[i] = newPos;
			}

			_instances.Add(divisions, this);
		}

		public Vector2[] GetDirections()
		{
			return _directions;
		}

		public Vector2 IndexToVector(int index)
		{
			return _directions[index];
		}

		public int VectorToIndex(Vector2 vector)
		{
			return _circlePointsIndex[vector];
		}
	}

	public static class VectorExtensions
	{
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