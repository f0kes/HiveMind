using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace AI.SteeringBehaviours
{
	public abstract class SteeringBehaviour
	{
		
		public abstract DesirabilityMap CalculateForPoint(Vector3 position, DesirabilityPoint point, int divisions);
	}
}