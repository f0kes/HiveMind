using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace AI.SteeringBehaviours
{
	public abstract class SteeringBehaviour
	{
		public DesirabilityMap Calculate(Vector3 position, DesirabilityPoint point, int divisions)
		{
			return CalculateForPoint(position, point, divisions);
		}
		protected abstract DesirabilityMap CalculateForPoint(Vector3 position, DesirabilityPoint point, int divisions);

		protected void AddDotProduct(DesirabilityMap map, Vector2 direction2D)
		{
			for (int i = 0; i < map.Divisions; i++)
			{
				var dir = map.Directions[i];
				var desirabilty = Vector2.Dot(dir, direction2D);
				if (desirabilty > 0)
				{
					map[i] += desirabilty;
				}
			}
		}
	}
}