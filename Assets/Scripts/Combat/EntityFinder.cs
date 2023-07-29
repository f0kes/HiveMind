using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

namespace Combat
{
	public static class EntityFinder
	{
		public static List<Entity> FindEntitiesInRadius(Vector3 point, float radius)
		{
			var all = GlobalEntities.GetAllEntitiesCopy();
			return all.Where(entity => Vector3.Distance(entity.transform.position, point) <= radius).ToList();
		}
	}
}