using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

namespace Characters
{
	public class WeaponContainer : MonoBehaviour
	{
		[SerializeField] private List<Weapon> _toAdd;
		[SerializeField] private int _maxItems = 6;
		protected  Dictionary<int, Weapon> Weapons = new Dictionary<int, Weapon>();
		public int MaxItems => _maxItems;
		private void Awake()
		{
			foreach (var weapon in _toAdd)
			{
				AddWeapon(Instantiate(weapon));
			}
		}

		public void RemoveAllItems()
		{
			Weapons.Clear();
		}

		public Weapon GetAnyWeapon()
		{
			return Weapons.Values.ToArray()[0];
		}

		public void AddWeapon(Weapon weapon, int index)
		{
			if (index < 0 || index >= _maxItems)
			{
				return;
			}

			Weapons[index] = weapon;
		}

		private void AddWeapon(Weapon weapon)
		{
			for (int i = 0; i < _maxItems; i++)
			{
				if (!Weapons.ContainsKey(i))
				{
					AddWeapon(weapon, i);
					return;
				}
				
			}
			Debug.Log("No free slots");
		}

		public Weapon GetWeapon(int index)
		{
			if (index < 0 || index >= _maxItems)
			{
				return null;
			}
			return Weapons.ContainsKey(index) ? Weapons[index] : null;
		}

		public Weapon GetWeapon(Weapon weapon)
		{
			return Weapons.Values.FirstOrDefault(w => w == weapon);
		}
		public List<Weapon> GetAllWeapons()
		{
			return Weapons.Values.ToList();
		}
	}
}