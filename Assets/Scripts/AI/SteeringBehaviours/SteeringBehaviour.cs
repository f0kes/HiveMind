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
	}
}