using Combat.ChargeSystem;
using Combat.Spells;
using Misc;

namespace Combat.CastSystem.Requirements
{
	public class ChargedRequirement : IRequirement<BaseSpell>
	{
		private ICharger _charger;
		public ChargedRequirement(ICharger charger) : base()
		{
			_charger = charger;
		}
		public TaskResult DoesSatisfy(BaseSpell spell)
		{
			return _charger.Charged(spell) ? TaskResult.Success : TaskResult.Failure("Spell is not charged");
		}
	}
}