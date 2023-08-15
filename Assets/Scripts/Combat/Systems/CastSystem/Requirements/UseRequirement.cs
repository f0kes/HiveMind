using Combat.Spells;
using Misc;

namespace Combat.CastSystem.Requirements
{
	public class UseRequirement : IRequirement<BaseSpell>
	{

		public TaskResult DoesSatisfy(BaseSpell spell)
		{
			var uses = spell.GetOwnerCharacter().GetUsesLeft();
			var infiniteUses = spell.IsInfinite;
			if(infiniteUses)
			{
				return TaskResult.Success;
			}
			else
			{
				return uses > 0 ? TaskResult.Success : TaskResult.Failure("No uses left");
			}
		}
	}
}