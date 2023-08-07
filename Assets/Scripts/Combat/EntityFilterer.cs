using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Characters;
using Combat.Spells;
using DefaultNamespace;
using Enums;
using UnityEngine;

namespace Combat
{
	public delegate bool EntityFilter(Entity filterer, Entity toFilter);

	public static class EntityFilterer
	{

		public static List<Entity> FilterEntitiesWithSpell(Entity filterer, IEnumerable<Entity> toFilter, BaseSpell spell)
		{
			return toFilter.Where(entity => spell.CanCastTarget(entity).ResultType == CastResultType.Success).ToList();
		}
		public static List<T> Filter<T>(this IEnumerable<T> toFilter, Entity filterer, EntityFilter filter) where T : Entity
		{
			return toFilter.Where(entity => filter(filterer, entity)).ToList();
		}

		public static EntityFilter And(this EntityFilter filter1, EntityFilter filter2) => (filterer, toFilter) => filter1(filterer, toFilter) && filter2(filterer, toFilter);

		public static EntityFilter Or(this EntityFilter filter1, EntityFilter filter2) => (filterer, toFilter) => filter1(filterer, toFilter) || filter2(filterer, toFilter);

		public static EntityFilter Not(this EntityFilter filter) => (filterer, toFilter) => !filter(filterer, toFilter);

		public static EntityFilter Xor(this EntityFilter filter1, EntityFilter filter2) => (filterer, toFilter) => filter1(filterer, toFilter) ^ filter2(filterer, toFilter);



		public static EntityFilter EnemyFilter = (filterer, toFilter) => filterer.Team != toFilter.Team;
		public static EntityFilter CharacterFilter = (_, toFilter) => toFilter is Character;

		public static EntityFilter TagFilter(EntityTag tag) => (_, toFilter) => toFilter.Tags.Contains(tag);

		public static EntityFilter NoneFilter = (_, _) => false;
		public static EntityFilter NotSelfFilter = (filterer, toFilter) => filterer != toFilter;
		public static EntityFilter NotDeadFilter = (_, toFilter) => !toFilter.IsDead;
		public static EntityFilter FriendlyFilter = (filterer, toFilter) => filterer.Team == toFilter.Team;

		public static EntityFilter ClassFilter(CharacterClass characterClass) => (_, toFilter) => toFilter is Character character && character.Class == characterClass;

		public static CastResult FilterEntity(Entity owner, Entity target, EntityFilter filter) //todo: filter description
		{
			if(filter(owner, target))
			{
				return new CastResult(CastResultType.Success);
			}
			else
			{
				return new CastResult(CastResultType.Fail, "Target does not match filter");
			}
		}



	}

}