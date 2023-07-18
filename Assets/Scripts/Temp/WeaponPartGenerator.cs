using System;
using System.Collections.Generic;
using Combat;
using Combat.Spells;
using UnityEngine;

namespace DefaultNamespace.Temp
{
	public class WeaponPartGenerator : MonoBehaviour
	{
		private SpellList SpellPool;

		private void Awake()
		{
			SpellPool = SpellInitializer.SpellPool;
		}
		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				var part = ModifierWeaponPartGenerator.GenerateWeaponPart(SpellPool, 100);
				part.PrintModifiers();
			}
		}
	}
}