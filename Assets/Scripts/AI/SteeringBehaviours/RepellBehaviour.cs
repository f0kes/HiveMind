using UnityEngine;

namespace AI.SteeringBehaviours
{
	public class RepellBehaviour : SteeringBehaviour
	{
		private bool _scaleWithDistance;
		public RepellBehaviour(bool scaleWithDistance = true)
		{
			_scaleWithDistance = scaleWithDistance;
		}
		protected override DesirabilityMap CalculateForPoint(Vector3 position, DesirabilityPoint point, int divisions)
		{
			DesirabilityMap map = new DesirabilityMap(divisions);
			var fromPoint = (position - (Vector3) point);
			var direction = fromPoint.normalized;
			if (_scaleWithDistance)
			{
				direction *= 1 / fromPoint.magnitude;
			}
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