using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
	public class EntityFactory
	{
		private Dictionary<Entity, Entity> _entityPrefabToInstance = new();

		public Entity CreateEntity(Entity entityPrefab)
		{
			var entity = Object.Instantiate(entityPrefab);
			_entityPrefabToInstance.Add(entityPrefab, entity);
			entity.gameObject.SetActive(false);
			return entity;
		}
		
		public List<Entity> GetAliveOriginals()
		{
			return (from kvp in _entityPrefabToInstance where !kvp.Value.IsDead select kvp.Key).ToList();
		}
		public void DestroyAll()
		{
			foreach(var entity in _entityPrefabToInstance.Values)
			{
				Object.Destroy(entity.gameObject);
			}
			_entityPrefabToInstance.Clear();
		}
		public bool IsDead(Entity entityPrefab)
		{
			return _entityPrefabToInstance[entityPrefab].IsDead;
		}
	}
}