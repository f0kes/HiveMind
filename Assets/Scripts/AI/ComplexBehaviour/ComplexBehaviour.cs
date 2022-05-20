using System.Collections.Generic;
using AI.SteeringBehaviours;
using DefaultNamespace;
using DefaultNamespace.AI;
using UnityEngine;

namespace AI.ComplexBehaviours
{
	public class ComplexBehaviour
	{
		public SteeringBehaviour BaseBehaviour { get; private set; }
		public PointGetter PointGetter { get; private set; }
		public float Weight { get; private set; }

		public ComplexBehaviour(PointGetter pointGetter, SteeringBehaviour baseBehaviour, float weight)
		{
			PointGetter = pointGetter;
			BaseBehaviour = baseBehaviour;
			Weight = weight;
		}

		public DesirabilityMap GetDesirabilities(Character entity, List<Character> other, int divisions)
		{
			var desirabilities = new DesirabilityMap(divisions);
			var points = PointGetter.GetPoints(entity, other);
			foreach (var point in points)
			{
				desirabilities += BaseBehaviour.CalculateForPoint(entity.transform.position, point, divisions);
			}

			desirabilities.Remap();
			return desirabilities * Weight;
		}
	}
}