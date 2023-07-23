using System.Collections.Generic;
using AI.SteeringBehaviours;
using DefaultNamespace.AI;
using UnityEngine;

namespace AI.ComplexBehaviours
{
	public class ComplexBehaviour
	{
		public SteeringBehaviour BaseBehaviour { get; private set; }
		public PointGetter PointGetter { get; private set; }
		public float Weight { get; private set; }

		//TODO move to pointDrawer
		private List<MeshRenderer> _drawnPoints = new List<MeshRenderer>();

		public ComplexBehaviour(PointGetter pointGetter, SteeringBehaviour baseBehaviour, float weight)
		{
			PointGetter = pointGetter;
			BaseBehaviour = baseBehaviour;
			Weight = weight;
		}

		public DesirabilityMap GetDesirabilities(Characters.Character entity, List<Characters.Character> other, int divisions )
		{
			var desirabilities = new DesirabilityMap(divisions);
			var points = GetDesirabilityPoints(entity, other);
		
			foreach (var point in points)
			{
				desirabilities += BaseBehaviour.Calculate(entity.transform.position, point, divisions);
			}

			desirabilities.Remap();
			return desirabilities * Weight;
		}

		public List<DesirabilityPoint> GetDesirabilityPoints(Characters.Character entity, List<Characters.Character> other)
		{
			var points = PointGetter.GetPoints(entity, other);
			return points;
		}
	}
}