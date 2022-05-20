using UnityEngine;

namespace AI.SteeringBehaviours
{
	public class StrafeBehaviour : SteeringBehaviour
	{
		public override DesirabilityMap CalculateForPoint(Vector3 position, DesirabilityPoint point, int divisions)
		{
			var map = new DesirabilityMap(divisions);
			if (((Vector3) point - position).magnitude <= 1f)
			{
				return map;
			}

			var direction = ((Vector3) point - position).normalized * point.Desirability;

			var left = new Vector2(-direction.z, direction.x);
			var right = new Vector2(direction.z, -direction.x);
			foreach (var kv in new DesirabilityMap(divisions).DirectionMap)
			{
				var desirabilty = Vector2.Dot(kv.Key, left);
				if (desirabilty > 0)
				{
					map[kv.Key] += desirabilty;
				}

				desirabilty = Vector2.Dot(kv.Key, right);
				if (desirabilty > 0)
				{
					map[kv.Key] += desirabilty;
				}
			}

			return map;
		}
	}
}