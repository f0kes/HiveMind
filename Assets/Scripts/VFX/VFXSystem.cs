using System;
using UnityEngine;

namespace VFX
{
	public class VFXSystem : MonoBehaviour
	{
		[SerializeField] private SpellIcon _spellIconPrefab;
		[SerializeField] private float _spellIconDuration;
		private static VFXSystem _instance;
		public static VFXSystem I => _instance;
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
	}
}