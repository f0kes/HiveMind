using UnityEngine;

namespace AI.SteeringBehaviours
{
	public class RepellBehaviour : SteeringBehaviour
	{
		private readonly bool _scaleWithDistance;
		private readonly float _repellDistance;

		public RepellBehaviour(bool scaleWithDistance = true, float repellDistance = 2f)
		{
			_scaleWithDistance = scaleWithDistance;
			_repellDistance = repellDistance;
		}

		protected override DesirabilityMap CalculateForPoint(Vector3 position, DesirabilityPoint point, int divisions)
		{
			DesirabilityMap map = new DesirabilityMap(divisions);
			var fromPoint = (position - (Vector3) point);
			var direction = fromPoint.normalized;
			if (_scaleWithDistance)
			{
				direction *= _repellDistance / fromPoint.magnitude;
			}

			Vector2 direction2D = new Vector2(direction.x, direction.z);
			AddDotProduct(map, direction2D);

			return map;
		}
	}
}