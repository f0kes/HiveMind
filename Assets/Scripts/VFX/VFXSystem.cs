using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VFX
{
	public class VFXSystem : MonoBehaviour
	{
		[SerializeField] private VFXData _data;
		[SerializeField] private SpellIcon _spellIconPrefab;
		[SerializeField] private float _spellIconDuration;
		[SerializeField] private int _poolSize = 35;
		private static VFXSystem _instance;
		public static VFXSystem I => _instance;
		public static VFXData Data => _instance._data;
		private List<VFXEffect> _pool = new List<VFXEffect>();

		private void Awake()
		{
			if(_instance != null)
			{
				Destroy(this);
				return;
			}
			_instance = this;
		}

		public void SpawnSpellIcon(Sprite icon, Transform target)
		{
			var spellIcon = Instantiate(_spellIconPrefab);
			spellIcon.SetIcon(icon);
			spellIcon.Follow(target);
			Destroy(spellIcon.gameObject, _spellIconDuration);
		}
		private T PlayEffect<T>(T effect, float duration = -1) where T : VFXEffect
		{
			if(effect == null)
			{
				Debug.LogError("Effect is null");
				return null;
			}
			var instance = Instantiate(effect);


			_pool.Insert(0, instance);
			var currentPoolSize = _pool.Count;
			if(currentPoolSize > _poolSize)
			{
				Destroy(_pool[^1].gameObject);
			}
			instance.OnDestroyEvent += e => _pool.Remove(e);
			if(duration > 0)
			{
				instance.SetDuration(duration);
			}
			if(instance.Duration > 0)
				Destroy(instance.gameObject, instance.Duration);
			return instance;
		}
		public VFXEffect PlayEffectFollow(VFXEffect effect, Transform target)
		{
			var instance = PlayEffect(effect);
			instance.Follow(target);
			return instance;
		}
		public VFXEffect PlayEffectFollow(VFXEffect effect, Transform target, float duration)
		{
			var instance = PlayEffect(effect, duration);
			instance.Follow(target);
			return instance;
		}
		public VFXEffect PlayEffectPoint(VFXEffect effect, Vector3 point)
		{
			var instance = PlayEffect(effect);
			instance.transform.position = point;
			return instance;
		}
		public VFXMultiplePointEffect PlayMultiplePointEffect(VFXMultiplePointEffect effect)
		{
			var instance = PlayEffect(effect);
			return instance;
		}
	}
}