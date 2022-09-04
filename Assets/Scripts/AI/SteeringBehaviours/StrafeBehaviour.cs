using UnityEngine;

namespace AI.SteeringBehaviours
{
	public class StrafeBehaviour : SteeringBehaviour
	{
		protected override DesirabilityMap CalculateForPoint(Vector3 position, DesirabilityPoint point, int divisions)
		{
			var map = new DesirabilityMap(divisions);
			if (((Vector3) point - position).magnitude <= 1f)
			{
				return map;
			}

			var direction = ((Vector3) point - position).normalized * point.Desirability;

			var left = new Vector2(-direction.z, direction.x);
			var right = new Vector2(direction.z, -direction.x);
			
			AddDotProduct(map, left);
			AddDotProduct(map, right);

			return map;
		}
	}
}