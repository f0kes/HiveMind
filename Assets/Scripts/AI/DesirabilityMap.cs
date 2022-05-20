using System.Collections.Generic;
using System.Linq;
using Tools;
using UnityEngine;

namespace AI
{
	public struct DesirabilityMap
	{
		public Dictionary<Vector3, float> DirectionMap { get; private set; }
		public int Divisions { get; private set; }

		public DesirabilityMap(int divisions)
		{
			DirectionMap = new Dictionary<Vector3, float>();
			var directions = CircleDivider.GetDivisions(divisions);
			foreach (Vector3 direction in directions)
			{
				DirectionMap.Add(direction, 0);
			}

			Divisions = divisions;
		}

		public DesirabilityMap(DesirabilityMap map)
		{
			DirectionMap = new Dictionary<Vector3, float>();
			var directions = CircleDivider.GetDivisions(map.Divisions);
			foreach (Vector3 direction in directions)
			{
				DirectionMap.Add(direction, 0);
			}

			Divisions = map.Divisions;
		}

		public float this[Vector3 direction]
		{
			get => DirectionMap[direction];
			set => DirectionMap[direction] = value;
		}

		// override plus
		public static DesirabilityMap operator +(DesirabilityMap a, DesirabilityMap b)
		{
			var result = new DesirabilityMap(a.Divisions);
			foreach (var direction in a.DirectionMap)
			{
				result[direction.Key] = a[direction.Key] + b[direction.Key];
			}

			return result;
		}

		public static DesirabilityMap operator *(DesirabilityMap a, float b)
		{
			var result = new DesirabilityMap(a.Divisions);
			foreach (var direction in a.DirectionMap)
			{
				result[direction.Key] = a[direction.Key] * b;
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