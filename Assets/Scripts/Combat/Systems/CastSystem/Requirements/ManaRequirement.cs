using Combat.Spells;
using Misc;

namespace Combat.CastSystem.Requirements
{
	public class ManaRequirement : IRequirement<BaseSpell>
	{
		public TaskResult DoesSatisfy(BaseSpell spell)
		{
			return spell.GetOwnerCharacter().CurrentMana >= spell.ManaCost ? TaskResult.Success : TaskResult.Failure("Not enough mana");
		}
	}
}