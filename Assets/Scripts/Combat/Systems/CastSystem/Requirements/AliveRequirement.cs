using Combat.Spells;
using Misc;
using UnityEngine;

namespace Combat.CastSystem.Requirements
{
	public class AliveRequirement : IRequirement<BaseSpell>
	{

		public TaskResult DoesSatisfy(BaseSpell spell)
		{
			if(spell == null)
			{
				Debug.LogError("alive requirement: spell is null");
				return TaskResult.Failure("Spell is null");
			}
			var character = spell.GetOwnerCharacter();
			if(character == null) return TaskResult.Failure("Character is null");
			var isAlive = !spell.GetOwnerCharacter().IsDead;
			return isAlive ? TaskResult.Success : TaskResult.Failure("Character is dead");
		}
	}
}