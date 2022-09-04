using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AI.SteeringBehaviours;
using Content;
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

		//TODO move to pointDrawer
		private List<MeshRenderer> _drawnPoints = new List<MeshRenderer>();

		public ComplexBehaviour(PointGetter pointGetter, SteeringBehaviour baseBehaviour, float weight)
		{
			PointGetter = pointGetter;
			BaseBehaviour = baseBehaviour;
			Weight = weight;
		}

		public DesirabilityMap GetDesirabilities(Character entity, List<Character> other, int divisions,
			bool drawPoints = false)
		{
			var desirabilities = new DesirabilityMap(divisions);
			var points = GetDesirabilityPoints(entity, other);
			if(drawPoints)
				DrawPoints(points);
			foreach (var point in points)
			{
				desirabilities += BaseBehaviour.Calculate(entity.transform.position, point, divisions);
			}

			desirabilities.Remap();
			return desirabilities * Weight;
		}

		public List<DesirabilityPoint> GetDesirabilityPoints(Character entity, List<Character> other)
		{
			var points = PointGetter.GetPoints(entity, other);
			return points;
		}

		private void DrawPoints(IEnumerable<DesirabilityPoint> points)
		{
			foreach (var point in _drawnPoints)
			{
				Object.Destroy(point.gameObject);
			}
			_drawnPoints.Clear();

			foreach (var point in points)
			{
				var pointObject = Object.Instantiate(ContentContainer.I.GetSphere());
				var transform = pointObject.transform;
				transform.position = point.Point;
				transform.localScale =
					new Vector3(point.Desirability, point.Desirability, point.Desirability);
				Color color;
				switch (BaseBehaviour)
				{
					case AttractBehaviour _:
						color = Color.green;
						break;
					case RepellBehaviour _:
						color = Color.red;
						break;
					default:
						color = Color.white;
						break;
				}

				color.a = 0.4f;
				pointObject.material.color = color;
				pointObject.name = BaseBehaviour.GetType().Name + " " + PointGetter.GetType().Name;
				_drawnPoints.Add(pointObject);
			}
		}
	}
}