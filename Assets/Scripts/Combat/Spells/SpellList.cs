using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

namespace Combat.Spells
{
	public class SpellList
	{
		public List<BaseSpell> SpellPool;
		public IEnumerable<BaseSpell> GetSpellByTags(SpellTag[] tags)
		{
			return (from spell in SpellPool
				let hasAllTags = tags.All(spellTag => spell.Tags.Contains(spellTag))
				where hasAllTags
				select spell);
		}


		public void Add(BaseSpell spell)
		{
			SpellPool.Add(spell);
		}
	}
}