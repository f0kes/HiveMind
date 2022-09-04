using System.Collections.Generic;
using System.Linq;
using Tools;
using UnityEngine;

namespace AI
{
	public class DesirabilityMap
	{
		// public static readonly Dictionary<int, Dictionary<Vector3, float>> DesirabilityMaps =
		// 	new Dictionary<int, Dictionary<Vector3, float>>();

		public Dictionary<Vector3, float> DirectionMap
		{
			get
			{
				Dictionary<Vector3, float> directionMap = new Dictionary<Vector3, float>();
				for (int i = 0; i < Divisions; i++)
				{
					Vector3 direction = _divider.IndexToVector(i);
					float value = _arrayMap[i];
					directionMap[direction] = value;
				}

				return directionMap;
			}
			private set
			{
				foreach (var kvp in value)
				{
					_arrayMap[_divider.VectorToIndex(kvp.Key)] = kvp.Value;
				}
			}
		}

		public int Divisions { get; private set; }
		private CircleDivider _divider;
		private float[] _arrayMap;
		public Vector2[] Directions => _divider.GetDirections();

		public DesirabilityMap(int divisions)
		{
			_arrayMap = new float[divisions];
			_divider = new CircleDivider(divisions);
			Divisions = divisions;
		}

		public float this[Vector3 direction]
		{
			get
			{
				int index = _divider.VectorToIndex(direction);
				return this[index];
			}
			set
			{
				int index = _divider.VectorToIndex(direction);
				this[index] = value;
			}
		}

		public float this[int index]
		{
			get => _arrayMap[index];
			set => _arrayMap[index] = value;
		}

		// override plus
		public static DesirabilityMap operator +(DesirabilityMap a, DesirabilityMap b)
		{
			var result = new DesirabilityMap(a.Divisions);
			for (int i = 0; i < a.Divisions; i++)
			{
				result[i] = a[i] + b[i];
			}

			return result;
		}

		public static DesirabilityMap operator *(DesirabilityMap a, float b)
		{
			var result = new DesirabilityMap(a.Divisions);
			for (int i = 0; i < a.Divisions; i++)
			{
				result[i] = a[i] * b;
			}

			return result;
		}

		public void Remap()
		{
			var max = DirectionMap.Values.Max();
			if (!(max >= 1))
				return;
			Dictionary<Vector3, float> remapped = new Dictionary<Vector3, float>();
			foreach (var pair in DirectionMap)
			{
				remapped[pair.Key] = pair.Value / max;
			}

			DirectionMap = remapped;
		}
	}
}