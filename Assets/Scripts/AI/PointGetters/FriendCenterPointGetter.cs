using System.Collections.Generic;
using System.Linq;
using AI;
using UnityEngine;

namespace DefaultNamespace.AI
{
	public class FriendCenterPointGetter : PointGetter
	{
		public override List<DesirabilityPoint> GetPoints(Characters.Character entity, List<Characters.Character> entities)
		{
			var points = new List<DesirabilityPoint>();
			var myPos = entity.transform.position;
			entities = entities.Where(e => e.Team == entity.Team).ToList();
			var friendCenter =
				entities.Aggregate(Vector3.zero, (current, friend) => current + friend.transform.position) /
				entities.Count;
			var distance = Vector3.Distance(myPos, friendCenter);
			points.Add(new DesirabilityPoint(friendCenter, distance));

			return points;
		}
	}
}