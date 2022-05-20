using System.Collections.Generic;
using System.Linq;
using AI;
using UnityEngine;

namespace DefaultNamespace.AI
{
	public class FriendPointGetter : PointGetter
	{
		private float _spaceToLeave;

		public FriendPointGetter(float spaceToLeave)
		{
			_spaceToLeave = spaceToLeave;
		}

		public override List<DesirabilityPoint> GetPoints(Character entity, List<Character> entities)
		{
			var points = new List<DesirabilityPoint>();
			var myPos = entity.transform.position;
			foreach (var e in entities.Where(e => entity.Team == e.Team))
			{
				var entityPos = e.transform.position;
				
				Vector3 path = entityPos - myPos;
				path = path.normalized * (path.magnitude - _spaceToLeave);
				var point = new DesirabilityPoint {Point = myPos + path, Desirability = e.AIDesirability};
				points.Add(myPos + path);
			}

			return points;
		}
	}
}