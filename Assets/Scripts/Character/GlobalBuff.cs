using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace Characters
{
	public class GlobalBuff : MonoBehaviour
	{
		[SerializeField] private float _healthBuff;

		private void Start()
		{
			var entities = EntityList.GetAllEntities();
			foreach (var entity in entities)
			{
				entity.SetMaxHealth(entity.MaxHealth + _healthBuff);
			}

			EntityList.OnEntityAdded += OnEntityAdded;
		}

		private void OnEntityAdded(ushort team, Entity entity)
		{
			entity.SetMaxHealth(entity.MaxHealth + _healthBuff);
		}
	}
}