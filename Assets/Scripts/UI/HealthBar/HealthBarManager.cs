using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace UI.HealthBars
{
	public class HealthBarManager : MonoBehaviour
	{
		[SerializeField] private HealthBar _healthBarPrefab;
		private Dictionary<Entity, HealthBar> _healthBars = new Dictionary<Entity, HealthBar>();
		private void Awake()
		{
			Entity.OnEntityCreated += OnEntityCreated;
			Entity.OnEntityDestroyed += OnEntityDestroyed;
		}
		private void OnDestroy()
		{
			Entity.OnEntityCreated -= OnEntityCreated;
			Entity.OnEntityDestroyed -= OnEntityDestroyed;
		}

		private void OnEntityCreated(Entity obj)
		{
			if(!_healthBars.ContainsKey(obj))
			{
				var hb = Instantiate(_healthBarPrefab, transform, false);
				hb.SetEntity(obj);
				_healthBars.Add(obj, hb);
			}
			else
			{
				Debug.LogError("Health bar already exists for entity " + obj.name);
			}
		}
		private void OnEntityDestroyed(Entity obj)
		{
			if(_healthBars.ContainsKey(obj))
			{
				Destroy(_healthBars[obj].gameObject);
				_healthBars.Remove(obj);
			}
			else
			{
				Debug.LogError("Health bar does not exist for entity " + obj.name);
			}
		}

	}
}