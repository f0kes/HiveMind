using Combat.Spells;
using Misc;

namespace Combat.CastSystem
{
	public interface ISpellInvoker
	{
		void AddRequirement(IRequirement<BaseSpell> castRequirement);

		void AddAction(IAction<BaseSpell> castAction);

		TaskResult Invoke(BaseSpell spell);

		TaskResult CanInvoke(BaseSpell spell);
	}
}