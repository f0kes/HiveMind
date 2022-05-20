using System.Collections.Generic;
using UnityEngine;

namespace AI.SteeringBehaviours
{
	public class AttractBehaviour : SteeringBehaviour
	{
		public override DesirabilityMap CalculateForPoint(Vector3 position, DesirabilityPoint point, int divisions)
		{
			DesirabilityMap map = new DesirabilityMap(divisions);
			if (((Vector3) point - position).magnitude <= 1f)
			{
				return map;
			}

			var direction = ((Vector3) point - position).normalized * point.Desirability;
			Vector2 direction2D = new Vector2(direction.x, direction.z);
			foreach (var kv in new DesirabilityMap(divisions).DirectionMap)
			{
				var desirabilty = Vector2.Dot(kv.Key, direction2D);
				if (desirabilty > 0)
				{
					map[kv.Key] += desirabilty;
				}
			}

			return map;
		}
	}
}