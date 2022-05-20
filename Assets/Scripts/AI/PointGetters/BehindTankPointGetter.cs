using System.Collections.Generic;
using System.Linq;
using AI;
using UnityEngine;

namespace DefaultNamespace.AI
{
	public class BehindTankPointGetter : PointGetter
	{
		private float _distanceToTank;

		public BehindTankPointGetter(float distanceToTank)
		{
			_distanceToTank = distanceToTank;
		}

		public override List<DesirabilityPoint> GetPoints(Character entity, List<Character> entities)
		{
			var points = new List<DesirabilityPoint>();
			//get friendly character with most desirability
			var tank = entities
				.Where(x => x.Team == entity.Team && x != entity)
				.OrderByDescending(x => x.AIDesirability)
				.FirstOrDefault();
			var enemyDamager = entities
				.Where(x => x.Team != entity.Team)
				.OrderByDescending(x => x.AIDesirability)
				.FirstOrDefault();
			if (tank == null || enemyDamager == null)
			{
				return points;
			}

			var averageDesirability = (tank.AIDesirability + enemyDamager.AIDesirability) / 2;

			var position = tank.transform.position;
			var dir = position - enemyDamager!.transform.position;
			var point = position + dir.normalized * _distanceToTank;

			points.Add(new DesirabilityPoint(point, averageDesirability));
			return points;
		}
	}
}