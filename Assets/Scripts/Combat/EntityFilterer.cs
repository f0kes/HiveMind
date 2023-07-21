using System.Collections.Generic;
using System.Linq;
using Combat.Spells;
using DefaultNamespace;
using Enums;

namespace Combat
{
	public static class EntityFilterer
	{
		public static CastResult FilterEntity(Entity filterer, Entity toFilter, TeamFilter targetTeam, EntityTag targetTags = EntityTag.Default)
		{
			if(targetTeam == TeamFilter.Both)
			{
				return toFilter.Tags.HasFlag(targetTags) ? CastResult.Success : new CastResult(CastResultType.Fail, "Target does not have required tags");
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

			return toFilter.Tags.HasFlag(targetTags) ? CastResult.Success : new CastResult(CastResultType.Fail, "Target does not have required tags");
		}
		public static List<Entity> FilterEntitiesWithSpell(Entity filterer, IEnumerable<Entity> toFilter, BaseSpell spell)
		{
			return toFilter.Where(entity => spell.CanCastTarget(entity).ResultType == CastResultType.Success).ToList();
		}
	}
}