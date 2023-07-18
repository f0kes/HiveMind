using System.Collections.Generic;
using System.Linq;
using Combat.Spells;
using Enums;
using Stats.Modifiers;
using Stats.Structures;
using UnityEngine;

namespace Combat
{
	public static class ModifierWeaponPartGenerator
	{
		public static WeaponPart GenerateWeaponPart(SpellList spellPool, float basePower)
		{
			//choose random spell tag 
			//get set of stats
			//randomly select stats
			//change power based on frequency of stat in spell pool
			//generate weapon part
			var part = WeaponPart.CreateDefault();

			var spellTag = GetRandomSpellTag(spellPool.SpellPool);
			var localSpellPool = spellPool.GetSpellByTags(new[] { spellTag });

			var baseSpells = localSpellPool as BaseSpell[] ?? localSpellPool.ToArray();

			var allUniqueStats = new HashSet<CS>
			(from spell in baseSpells
				from stat in spell.DependantStats
				select stat);

			var randomStat = allUniqueStats.ToArray()[Random.Range(0, allUniqueStats.Count)];
			var power = basePower / CalculateStatFrequency(randomStat, baseSpells);
			var modifier = ScriptableObject.CreateInstance<StatModifierAdd>();
			modifier.Value = power;
			part.Modifiers.Add(new WeaponPart.EnumeratedModifier { Stat = randomStat, Value = modifier, Filter = spellTag});
			return part;
		}
		private static float CalculateStatFrequency(CS stat, IEnumerable<BaseSpell> spellPool)
		{
			return spellPool.Count(spell => spell.DependantStats.Contains(stat));
		}
		private static SpellTag GetRandomSpellTag(List<BaseSpell> spellPool)
		{
			var tags = new List<SpellTag>();
			foreach(var spell in spellPool)
			{
				foreach(var tag in spell.Tags)
				{
					if(!tags.Contains(tag))
						tags.Add(tag);
				}
			}
			return tags[Random.Range(0, tags.Count)];
		}
	}
}