using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace DefaultNamespace.UI
{
	public class WeaponContainerUI : MonoBehaviour
	{
		[SerializeField] private WeaponIcon _weaponIconPrefab;
		private List<WeaponIcon> _weaponIcons = new List<WeaponIcon>();

		public void ShowWeaponContainer(WeaponContainer container)
		{
			foreach (var weapon in _weaponIcons)
			{
				Destroy(weapon.gameObject);
			}

			_weaponIcons = new List<WeaponIcon>();
			for (int i = 0; i < container.MaxItems; i++)
			{
				var weaponIcon = Instantiate(_weaponIconPrefab, transform);
				_weaponIcons.Add(weaponIcon);
			}

			for (int i = 0; i < _weaponIcons.Count; i++)
			{
				_weaponIcons[i].SetWeapon(container.GetWeapon(i));
			}
		}
	}
}