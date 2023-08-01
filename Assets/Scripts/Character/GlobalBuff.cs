using System;
using System.Collections.Generic;
using DefaultNamespace;
using GameState;
using UnityEngine;

namespace Characters
{
	public class GlobalBuff : MonoBehaviour
	{
		[SerializeField] private float _healthBuff;
		[SerializeField] private float _healthBuffMultiplier;
		private List<Entity> _buffedEntities = new List<Entity>();


		private void Start()
		{
			var entities = GameStateController.Battle.EntityRegistry.GetAllEntitiesCopy();
			foreach (var entity in entities)
			{
				BuffEntity(entity);
			}

			GameStateController.Battle.EntityRegistry.OnEntityAdded += OnEntityAdded;
		}

		private void OnEntityAdded(ushort team, Entity entity)
		{
			BuffEntity(entity);
		}

		private void BuffEntity(Entity entity)
		{
			if (_buffedEntities.Contains(entity))
				return;
			
			_buffedEntities.Add(entity);
			entity.SetMaxHealth(entity.MaxHealth * _healthBuffMultiplier + _healthBuff);
		}
	}
}