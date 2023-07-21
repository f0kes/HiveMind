using System.Collections.Generic;
using AI;
using Tools;
using UnityEngine;

namespace DefaultNamespace.AI
{
	public class WallGetter : PointGetter
	{
		private float _viewDistance;
		private int _divisions;

		public WallGetter(float viewDistance, int divisions)
		{
			_viewDistance = viewDistance;
			_divisions = divisions;
		}

		public override List<DesirabilityPoint> GetPoints(Characters.Character entity, List<Characters.Character> entities)
		{
			var map = new DesirabilityMap(_divisions);
			var points = new List<DesirabilityPoint>();
			var position = entity.transform.position + Vector3.up;
			foreach (var vector in map.Directions)
			{
				//raycast to see if there is a wall
				RaycastHit hit;
				if (Physics.Raycast(position, vector.XZ(), out hit, _viewDistance))
				{
					DesirabilityPoint point = new DesirabilityPoint();
					if (hit.collider.gameObject != entity.gameObject)
					{
						 point = new DesirabilityPoint() {Point = hit.point, Desirability = 1};
					}

					points.Add(point);
				
				}
			}

			return points;
		}
	}
}