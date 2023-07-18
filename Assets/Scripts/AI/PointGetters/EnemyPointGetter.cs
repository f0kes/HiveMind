using System.Collections.Generic;
using System.Linq;
using AI;
using UnityEngine;

namespace DefaultNamespace.AI
{
	public class EnemyPointGetter : PointGetter
	{
		private float _spaceToLeave;
		private bool _chaseThreat;

		public EnemyPointGetter(float spaceToLeave, bool chaseThreat = false)
		{
			_spaceToLeave = spaceToLeave;
			_chaseThreat = chaseThreat;
		}

		public override List<DesirabilityPoint> GetPoints(Character.Character entity, List<Character.Character> entities)
		{
			var points = new List<DesirabilityPoint>();
			var myPos = entity.transform.position;
			foreach (var e in entities.Where(e => entity.Team != e.Team))
			{
				float desirability = _chaseThreat ? e.AIThreat : e.AIDesirability;
				var entityPos = e.transform.position;

				var path = entityPos - myPos;
				path = path.normalized * (path.magnitude - _spaceToLeave);
				var point = new DesirabilityPoint(myPos + path, desirability);
				points.Add(point);
			}

			return points;
		}
	}
}