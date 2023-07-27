using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace Combat
{
	public static class EntityFinder
	{
		public static List<Entity> FindEntitiesInRadius(Vector3 point, float radius)
		{
			var all = GlobalEntities.GetAllEntitiesCopy();
			var result = new List<Entity>();
			foreach (var entity in all)
			{
				if (Vector3.Distance(entity.transform.position, point) <= radius)
				{
					result.Add(entity);
				}
			}
			return result;
		}
	}
}