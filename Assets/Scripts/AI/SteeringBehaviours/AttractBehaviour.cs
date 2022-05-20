using System.Collections.Generic;
using UnityEngine;

namespace AI.SteeringBehaviours
{
	public class AttractBehaviour : SteeringBehaviour
	{
		private bool _scaleWithDistance;

		public AttractBehaviour(bool scaleWithDistance = true)
		{
			_scaleWithDistance = scaleWithDistance;
		}

		protected override DesirabilityMap CalculateForPoint(Vector3 position, DesirabilityPoint point, int divisions)
		{
			var map = new DesirabilityMap(divisions);
			var toPoint = (Vector3) point - position;
			if (toPoint.magnitude <= 1f)
			{
				return map;
			}

			var direction = toPoint.normalized * point.Desirability;
			if (_scaleWithDistance)
			{
				direction *= toPoint.magnitude;
			}
			var direction2D = new Vector2(direction.x, direction.z);
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