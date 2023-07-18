using System;
using System.Collections.Generic;
using Combat.Spells;
using Enums;
using Stats;
using Stats.Modifiers;
using Stats.Structures;
using UnityEngine;

namespace Combat
{
	[CreateAssetMenu(fileName = "New Weapon Part", menuName = "Weapon Part")]
	public class WeaponPart : ScriptableObject
	{
		[Serializable]
		public struct EnumeratedModifier
		{
			public CS Stat;
			public StatModifier Value;
			public SpellTag Filter;
		}
		public List<EnumeratedModifier> Modifiers;
		public List<BaseSpell> Spells;
		public List<BaseEffect> Effects;
		
		//TODO: Description
		public static WeaponPart CreateDefault()
		{
			var part = CreateInstance<WeaponPart>();
			part.Modifiers = new List<EnumeratedModifier>();
			part.Spells = new List<BaseSpell>();
			part.Spells.Add(BaseSpell.CreateDefault());
			part.Effects = new List<BaseEffect>();
			return part;
		}
		
		public void PrintModifiers()
		{
			foreach(var modifier in Modifiers)
			{
				Debug.Log(modifier.Stat + " " + modifier.Value);
			}
		}

	}
}