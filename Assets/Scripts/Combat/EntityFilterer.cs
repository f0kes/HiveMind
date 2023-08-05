using System.Collections.Generic;
using System.Linq;
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
		public static CastResult FilterEntity(Entity filterer, Entity toFilter, TeamFilter targetTeam, EntityTag targetTag = EntityTag.Default)
		{
			if(targetTeam == TeamFilter.Both)
			{
				return toFilter.Tags.Contains(targetTag) ? CastResult.Success : new CastResult(CastResultType.Fail, "Target does not have required tags");
			}
			if(filterer.Team == toFilter.Team)
			{
				if(targetTeam == TeamFilter.Enemy)
				{
					return new CastResult(CastResultType.Fail, "Target is friendly");
				}
			}
			else
			{
				if(targetTeam == TeamFilter.Friendly)
				{
					return new CastResult(CastResultType.Fail, "Target is enemy");
				}
			}

			return toFilter.Tags.Contains(targetTag) ? CastResult.Success : new CastResult(CastResultType.Fail, "Target does not have required tags");
		}
		public static List<Entity> FilterEntitiesWithSpell(Entity filterer, IEnumerable<Entity> toFilter, BaseSpell spell)
		{
			return toFilter.Where(entity => spell.CanCastTarget(entity).ResultType == CastResultType.Success).ToList();
		}

		public static EntityFilter EnemyFilter = (filterer, toFilter) => filterer.Team != toFilter.Team;
		public static EntityFilter CharacterFilter = (_, toFilter) => toFilter is Character;

		public static EntityFilter TagFilter(EntityTag tag) => (_, toFilter) => toFilter.Tags.Contains(tag);

		public static EntityFilter NoneFilter = (_, _) => false;
		public static EntityFilter NotSelfFilter = (filterer, toFilter) => filterer != toFilter;

	}
}