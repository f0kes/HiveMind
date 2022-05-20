using UnityEngine;

namespace AI.SteeringBehaviours
{
	public class RepellBehaviour : SteeringBehaviour
	{
		public override DesirabilityMap CalculateForPoint(Vector3 position, DesirabilityPoint point, int divisions)
		{
			DesirabilityMap map = new DesirabilityMap(divisions);
			var direction = (position - (Vector3) point).normalized;
			Vector2 direction2D = new Vector2(direction.x, direction.z);
			foreach (var kv in new DesirabilityMap(divisions).DirectionMap)
			{
				float desirabilty = Vector2.Dot(kv.Key, direction2D);
				if (desirabilty > 0)
				{
					map[kv.Key] += desirabilty;
				}
			}

			return map;
		}
	}
}